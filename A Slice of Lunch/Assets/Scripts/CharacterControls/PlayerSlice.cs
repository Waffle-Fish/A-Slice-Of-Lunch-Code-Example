using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSlice : MonoBehaviour
{
    readonly private Vector3 INVALID_VECTOR = new Vector3(999999, 999999, 999999);

    [Header("Settings")]
    [SerializeField]
    private int maxSlices = 2;

    [Header("Slicing")]
    [SerializeField][Min(0f)] private float minFoodAreaSize = 1f;
    [Tooltip("Number of slices made")]
    private int currentSlicesLeft = 0;
    private Vector3[] sliceEndPoints = new Vector3[2];
    private LineRenderer sliceMarking;
    private Vector3 mouseWorldPos;
    enum Directions {left, right, above, below, on}
    UndoManager undoManager;

    [Header("Object Pooling")]
    private ObjectPooler maskPool;
    private ObjectPooler foodPool;

    public static event Action<TurnActions> OnSliceFinish;
    public static event Action<int> OnSliceCountChange;
    private TurnActions movesMadeThisTurn = new();
    private List<GameObject> foodsToDisableThisTurn;

    private PlayerInputActions.PlayerActions playerActions;
    
    private void Awake() {
        sliceMarking = GetComponent<LineRenderer>();
        undoManager = GetComponent<UndoManager>();
    }

    private void Start() {
        playerActions = PlayerInputManager.Instance.PlayerActions;
        sliceEndPoints[0] = INVALID_VECTOR;
        sliceEndPoints[1] = INVALID_VECTOR;

        currentSlicesLeft = maxSlices;
        movesMadeThisTurn.slicesModifiedThisTurn = new();
        movesMadeThisTurn.foodsToDisableThisTurn = new();

        OnSliceCountChange?.Invoke(currentSlicesLeft);
    }

    private void Update() {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(PlayerInputManager.Instance.MousePos);
        DetectRightClick();
        DisplaySliceMarkings();
    }

    private void DetectLeftClick()
    {
        if (playerActions.LeftClick.WasPressedThisFrame()) BeginSlice();
        if (playerActions.LeftClick.WasReleasedThisFrame()) FinalizeSlice();
    }

    private void DetectRightClick()
    {
        if (playerActions.RightClick.WasPressedThisFrame()) BeginSlice();
        if (playerActions.RightClick.WasReleasedThisFrame()) FinalizeSlice();
    }

    private void BeginSlice() {
        if (currentSlicesLeft == 0) return;

        // return if slice starts on food
        Ray clickRay = Camera.main.ScreenPointToRay(playerActions.MousePosition.ReadValue<Vector2>());
        Debug.DrawRay(clickRay.origin, clickRay.direction, Color.cyan, 100f);
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(clickRay);
        if (hit2D.collider != null && (hit2D.collider.CompareTag("Food") || hit2D.collider.CompareTag("NoSlice"))) return;

        sliceEndPoints[0] = mouseWorldPos; 
    }

    private void FinalizeSlice() {
        // Variables
        List<RaycastHit2D> slicedObjects = new();
        
        // Base Conditions
        if (sliceEndPoints[0] == INVALID_VECTOR) return;
        if (currentSlicesLeft == 0) return;
        
        Ray clickRay = Camera.main.ScreenPointToRay(playerActions.MousePosition.ReadValue<Vector2>());
        
        Debug.DrawRay(clickRay.origin, clickRay.direction, Color.cyan, 100f);
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(clickRay);
        if (hit2D.collider != null && (hit2D.collider.CompareTag("Food") || hit2D.collider.CompareTag("NoSlice"))) {
            sliceEndPoints[0] = INVALID_VECTOR;
            return;
        }

        sliceEndPoints[1] = mouseWorldPos;
        slicedObjects = Physics2D.LinecastAll(sliceEndPoints[0], sliceEndPoints[1]).ToList();

        slicedObjects.RemoveAll(slice => !slice.collider.CompareTag("Food"));

        if (slicedObjects.Count == 0) {
            Reset();
            return;
        }

        // Setup Undo
        movesMadeThisTurn.slicesModifiedThisTurn = new();
        movesMadeThisTurn.foodsToDisableThisTurn = new();

        // Process Slice for each piece
        foreach (var foodCollider in slicedObjects) {
            if (!foodCollider.transform.CompareTag("Food")) continue;
            SliceFood(foodCollider);
        }

        OnSliceFinish?.Invoke(movesMadeThisTurn);
        WinManager.Instance.UpdateTotalFoodList();
        UpdateCurrentSlicesCount(-1);
        Reset();
        
        if (!ValidateFoodSizes()) {
            Debug.Log("A food slice was too small! Canceling slice");
            undoManager.UndoSlice();
            return;
        }

        AudioManager.Instance.PlaySFX("Slice");
    }

    private void SliceFood(RaycastHit2D foodCollider) {
        float separationSpace = 0.05f;

        Vector3[] sliceEdgePoints = new Vector3[2];
        Transform parentFood = foodCollider.transform.parent;
        
        maskPool = foodCollider.collider.GetComponent<ObjectPooler>();
        foodPool = parentFood.parent.GetComponent<ObjectPooler>();

        SliceObjectData firstSliceData = new();
        SliceObjectData secondSliceData = new();
        
        int originalLayer = foodCollider.collider.gameObject.layer;
        int sliceLayer = LayerMask.NameToLayer("Slice");
        int sliceLayerMask = LayerMask.GetMask("Slice");
        foodCollider.collider.gameObject.layer = sliceLayer;

        sliceEdgePoints[0] = Physics2D.Raycast(sliceEndPoints[0], (sliceEndPoints[1] - sliceEndPoints[0]), distance: 100f, layerMask: sliceLayerMask).point;
        sliceEdgePoints[1] = Physics2D.Raycast(sliceEndPoints[1], (sliceEndPoints[0] - sliceEndPoints[1]), distance: 100f, layerMask: sliceLayerMask).point;
        Vector2 sliceCenter = (sliceEdgePoints[0] + sliceEdgePoints[1]) / 2f;
        foodCollider.collider.gameObject.layer = originalLayer;
        
        Debug.DrawLine(sliceEdgePoints[0], sliceEdgePoints[1], Color.black, 100f);

        // Rotate mask to be parallel to slice
        bool rotateFromYAxis = (sliceEndPoints[0].x < sliceEndPoints[1].x && sliceEndPoints[0].y > sliceEndPoints[1].y) || (sliceEndPoints[1].x < sliceEndPoints[0].x && sliceEndPoints[1].y > sliceEndPoints[0].y);
        float opposite = (rotateFromYAxis) ? Mathf.Abs(sliceEndPoints[1].x - sliceEndPoints[0].x) : Mathf.Abs(sliceEndPoints[1].y - sliceEndPoints[0].y);
        float hypotenuse = Vector2.Distance(sliceEndPoints[0], sliceEndPoints[1]);
        float rotAng = Mathf.Asin(opposite / hypotenuse) * Mathf.Rad2Deg;

        // Get perpendicular vector
        Vector2 perpendicularSlice = Vector2.Perpendicular(sliceEndPoints[0]-sliceEndPoints[1]).normalized;

        // Spawn Mask
        GameObject spriteMaskObj = maskPool.GetPooledObject();
        SpriteMask spriteMask = spriteMaskObj.GetComponent<SpriteMask>();
        Vector2 maskPos = sliceCenter + spriteMaskObj.transform.localScale.x /2f * perpendicularSlice;
        spriteMaskObj.SetActive(true);
        spriteMaskObj.transform.SetPositionAndRotation(maskPos, Quaternion.Euler(0,0,rotAng));
        firstSliceData.spriteMaskObj = spriteMaskObj;
        
        // Update polygonCollider2D
        PolygonCollider2D originalFoodCollider = (PolygonCollider2D)foodCollider.collider;
        firstSliceData.originalPolyColPoints = originalFoodCollider.points;
        firstSliceData.polygonCollider2D = originalFoodCollider;
        List<Vector2> originalFoodColliderPoints = originalFoodCollider.points.ToList();
        originalFoodCollider.SetPath(0,GenerateNewSlicePoints(originalFoodCollider, sliceEdgePoints, GetSidePointIsOn(maskPos)));

        // WORK ON OTHER SLICE
        GameObject otherSlice = foodPool.GetPooledObject();
        otherSlice.transform.SetPositionAndRotation(parentFood.position,parentFood.rotation);

        // Update masks of other slice
        int maskIndex = 0;
        ObjectPooler otherSliceMaskPool = otherSlice.transform.GetChild(0).GetComponent<ObjectPooler>();
        GameObject currentMask = maskPool.GetPooledObjectAtIndex(maskIndex);
        int numMasks = maskPool.GetNumObjectsActive();
        while (maskIndex < numMasks-1) {
            GameObject prevSliceMask = otherSliceMaskPool.GetPooledObject();
            prevSliceMask.SetActive(true);
            prevSliceMask.transform.SetPositionAndRotation(currentMask.transform.position, currentMask.transform.rotation);
            maskIndex++;
            currentMask = maskPool.GetPooledObjectAtIndex(maskIndex);
        }
        GameObject finalSliceMask = otherSliceMaskPool.GetPooledObject();
        finalSliceMask.SetActive(true);
        Vector2 otherSliceSpawnPos = sliceCenter - spriteMaskObj.transform.localScale.x / 2f * perpendicularSlice;
        finalSliceMask.transform.SetPositionAndRotation(otherSliceSpawnPos,Quaternion.Euler(0,0,rotAng));
        secondSliceData.spriteMaskObj = finalSliceMask;
        
        // Update other slice polygonCollider2D
        PolygonCollider2D otherSliceNewFoodCollider = otherSlice.GetComponentInChildren<PolygonCollider2D>();
        secondSliceData.originalPolyColPoints = otherSliceNewFoodCollider.points;
        secondSliceData.polygonCollider2D = otherSliceNewFoodCollider;
        otherSliceNewFoodCollider.SetPath(0, originalFoodColliderPoints);
        otherSliceNewFoodCollider.SetPath(0,GenerateNewSlicePoints(otherSliceNewFoodCollider, sliceEdgePoints, GetSidePointIsOn(otherSliceSpawnPos)));

        // Separate Slices
        parentFood.Translate(-perpendicularSlice * separationSpace);
        otherSlice.transform.Translate(perpendicularSlice * separationSpace);
        otherSlice.SetActive(true);

        movesMadeThisTurn.foodsToDisableThisTurn.Add(otherSlice);
        movesMadeThisTurn.slicesModifiedThisTurn.Add(firstSliceData);
        movesMadeThisTurn.slicesModifiedThisTurn.Add(secondSliceData);

        // Debug.Log(otherSliceNewFoodCollider.transform.parent.name + " " + otherSliceNewFoodCollider.bounds.size.x * otherSliceNewFoodCollider.bounds.size.y);
    }

    private bool ValidateFoodSizes()
    {
        bool FoodIsSmall(Vector3 dimension) { return dimension.x * dimension.y < minFoodAreaSize;}
        foreach (var sliceData in movesMadeThisTurn.slicesModifiedThisTurn)
        {
            // Debug.Log(sliceData.spriteMaskObj.transform.parent.name + ": " + sliceData.polygonCollider2D.bounds.size.x * sliceData.polygonCollider2D.bounds.size.y);
            if (FoodIsSmall(sliceData.polygonCollider2D.bounds.size)) return false;
        }
        return true;
    }
    
    private List<Vector2> GenerateNewSlicePoints(PolygonCollider2D foodCollider, Vector3[] sliceEdgePoints, Tuple<Directions, Directions> sideMaskIsOn){
        // Sutherland-Hodgam algorithm
        List<Vector2> inputPoints = new(foodCollider.points);
        List<Vector2> outputPoints = new();

        for(int i = 0; i < inputPoints.Count; i++)
        {
            Vector2 currentPoint = inputPoints[i];
            Vector2 prevPoint = inputPoints[(i-1 < 0) ? ^1 : i-1];

            // points in polycollider are affected by scale, divide by lossyScale to get point in world
            Vector2 worldPosCurPoint = currentPoint / foodCollider.transform.lossyScale.x + (Vector2)foodCollider.transform.position;
            Vector2 worldPosPrevPoint = prevPoint / foodCollider.transform.lossyScale.x + (Vector2)foodCollider.transform.position;
            Vector2 newEdgePoint1 = sliceEdgePoints[0] - foodCollider.transform.position;
            Vector2 newEdgePoint2 = sliceEdgePoints[1] - foodCollider.transform.position;

            // Get intersection in local scale
            Vector2 intersectingPoint = GetTwoLinesIntersectPoint(currentPoint, prevPoint, newEdgePoint1, newEdgePoint2);

            Tuple<Directions, Directions> sideCurPointIsOn = GetSidePointIsOn(worldPosCurPoint);
            Tuple<Directions, Directions> sidePrevPointIsOn = GetSidePointIsOn(worldPosPrevPoint);
            if (sideCurPointIsOn.Item1 != sideMaskIsOn.Item1 || sideCurPointIsOn.Item2 != sideMaskIsOn.Item2) {
                if ((sidePrevPointIsOn.Item1 == sideMaskIsOn.Item1 && sidePrevPointIsOn.Item1 != Directions.on) || (sidePrevPointIsOn.Item2 == sideMaskIsOn.Item2 && sidePrevPointIsOn.Item2 != Directions.on)) {
                    outputPoints.Add(intersectingPoint);
                }
                outputPoints.Add(currentPoint);
            } 
            else if (sidePrevPointIsOn.Item1 != sideMaskIsOn.Item1 || sidePrevPointIsOn.Item2 != sideMaskIsOn.Item2)
            {
                outputPoints.Add(intersectingPoint);
            }
        }
        return outputPoints;
    }

    private void DisplaySliceMarkings()
    {
        if (sliceEndPoints[0] == INVALID_VECTOR) {
            sliceMarking.enabled = false;
            return;
        }
        sliceMarking.enabled = true;
        Vector3[] smPos = new Vector3[2];
        smPos[0] = sliceEndPoints[0];
        smPos[1] = mouseWorldPos;
        sliceMarking.SetPositions(smPos);
    }

    private void Reset() {
        sliceEndPoints[0] = INVALID_VECTOR;
        sliceEndPoints[1] = INVALID_VECTOR;
    }

    private Tuple<Directions, Directions> GetSidePointIsOn(Vector2 point) {
        Directions xDir = Directions.on;
        Directions yDir = Directions.on;

        float numerator = sliceEndPoints[1].y - sliceEndPoints[0].y;
        float denominator = sliceEndPoints[1].x - sliceEndPoints[0].x;
        if (Mathf.Approximately(numerator,0)) {
            yDir = (point.y > sliceEndPoints[0].y) ? Directions.above : Directions.below;
            return new(xDir, yDir);
        }

        if (Mathf.Approximately(denominator,0)) {
            xDir = (point.x > sliceEndPoints[0].x) ? Directions.right : Directions.left;
            return new(xDir, yDir);
        }

        float slope = numerator / denominator;
        float yLine = GetSlopeIntercept(sliceEndPoints[0], sliceEndPoints[1], point.x);
        float yDelta = yLine - point.y;
        if (yDelta != 0) yDir = (yDelta > 0 ) ? Directions.below : Directions.above;
        if (slope != 0) xDir = ((slope > 0 && yDir == Directions.above) || (slope < 0 && yDir == Directions.below)) ? Directions.right :Directions.left; 
        return new(xDir, yDir);
    }

    private float GetSlopeIntercept(Vector2 a, Vector2 b, float x) {
        float slope = (a.y - b.y) / (a.x - b.x);
        float yIntercept = a.y - slope * a.x;
        return slope * x + yIntercept;
    }

    private Vector2 GetTwoLinesIntersectPoint(Vector2 line1_A, Vector2 line1_B, Vector2 line2_A, Vector2 line2_B) {
        float slope1 = (line1_A.y - line1_B.y) / (line1_A.x - line1_B.x);
        float yIntercept1 = line1_A.y - slope1 * line1_A.x;

        float slope2 = (line2_A.y - line2_B.y) / (line2_A.x - line2_B.x);
        float yIntercept2 = line2_A.y - slope2 * line2_A.x;

        float x;
        float y;
        if (line1_A.x - line1_B.x == 0) {
            x = line1_A.x;
            y = slope2 * x + yIntercept2;
        } else if (line2_A.x - line2_B.x == 0) {
            x = line2_A.x;
            y = slope1 * x + yIntercept1;
        }
        else {
            x = (yIntercept2 - yIntercept1) / (slope1 - slope2);
            y = slope1 * x + yIntercept1;
        }
        return new(x, y);
    }

    public void UpdateCurrentSlicesCount(int val) {
        currentSlicesLeft += val;
        currentSlicesLeft = math.clamp(currentSlicesLeft, 0, maxSlices);
        OnSliceCountChange?.Invoke(currentSlicesLeft);
    }
}
