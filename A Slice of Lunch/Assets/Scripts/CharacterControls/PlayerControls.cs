using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Rename this to SliceControls
public class PlayerControls : MonoBehaviour
{
    [Header("Detect Object")]
    private Vector3 mouseWorldPosition;

    [Header("Slice Variables")]
    // [SerializeField]
    // private GameObject spriteMask;
    public bool IsHoldingKnife /*{ get; private set; }*/ = false;
    [SerializeField]
    private int maxSlicesCount = 2;
    [Tooltip("Number of slices made")]
    private int currentSlicesLeft = 0;
    readonly private Vector3 CHECK_VECTOR = new Vector3(999999, 999999, 999999);
    private Vector3[] slicePoints = new Vector3[2];
    private List<RaycastHit2D> slicedObjects = new();
    Vector2 sliceEdgePoint_0;
    Vector2 sliceEdgePoint_1;

    [Header("Slice Indicators")]
    [SerializeField]
    private GameObject endPointObj;
    [SerializeField]
    private float endPointPosZ;
    private List<GameObject> endPoints;
    private LineRenderer sliceMarking;

    [Header("Object Pooling")]
    private ObjectPooler maskPool;
    private ObjectPooler foodPool;

    [Header("Undo")]
    int currentUndoIndex = 0;
    private Stack<List<GameObject>> everyMoveGameObject = new();
    private Stack<List<PolygonCollider2D>> everyMovePolygonColliders = new();

    [Header("HUD")]
    [SerializeField]
    private TextMeshProUGUI slicesText;

    // [Header("Update PolygonColliders")]
    enum Directions {left, right, above, below, on}

    Tuple<Directions, Directions> sliceDir;


    private void Awake() {
        sliceMarking = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        mouseWorldPosition = new();
        endPoints = new();

        for (int i = 0; i < 2; i++) {
            endPoints.Add(Instantiate(endPointObj, transform.position, transform.rotation, transform));
        }
        ResetSlicePoints();
        UpdateCurrentSlicesCount(maxSlicesCount);
    }

    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DetectLeftClick();
        DisplaySliceMarkings();
    }

    private void DisplaySliceMarkings()
    {
        if (slicePoints[0] == CHECK_VECTOR) return;

        Vector3[] smPos = new Vector3[2];
        smPos[0] = slicePoints[0];
        smPos[1] = mouseWorldPosition;
        // Making it behind the circle of the slice pos
        smPos[0].z = endPoints[0].transform.position.z + 1;
        smPos[1].z = endPoints[1].transform.position.z + 1;
        sliceMarking.SetPositions(smPos);

        Vector3 endPointPos = mouseWorldPosition;
        endPointPos.z = endPointPosZ;
        endPoints[1].transform.position = endPointPos;
    }

    private void DetectLeftClick()
    {
        if (!IsHoldingKnife) return;
        if (Mouse.current.leftButton.wasPressedThisFrame) BeginSlice();
        if (Mouse.current.leftButton.wasReleasedThisFrame) FinalizeSlice();
    }

    private void ResetSlicePoints()
    {
        slicePoints[0] = CHECK_VECTOR;
        slicePoints[1] = CHECK_VECTOR;

        foreach (var ep in endPoints) ep.SetActive(false);
        sliceMarking.SetPositions(slicePoints);
    }

    private void BeginSlice() {
        if (currentSlicesLeft == 0) return;

        // return if slice starts on food
        Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D =  Physics2D.GetRayIntersection(clickRay);
        if (hit2D.collider != null && hit2D.collider.CompareTag("Food")) return;

        slicePoints[0] = mouseWorldPosition;
        foreach (var ep in endPoints) ep.SetActive(true);
        endPoints[0].transform.position = new (slicePoints[0].x, slicePoints[0].y, endPointPosZ);
    }

    private void FinalizeSlice() {
        // Base conditions
        if (slicePoints[0] == CHECK_VECTOR) return;
        if (currentSlicesLeft == 0) return;
        slicePoints[1] = mouseWorldPosition;
        slicedObjects = Physics2D.LinecastAll(slicePoints[0], slicePoints[1]).ToList();
        if (slicedObjects.Count == 0) {
            ResetSlicePoints();
            return;
        }

        // Initialize Undo 
        List<GameObject> objectsEnabledThisTurn = new();
        List<PolygonCollider2D> originalPolygonColliders = new();
        currentUndoIndex++;

        foreach (var foodCollider in slicedObjects) {
            // ignores non food in slicedObjects
            if (!foodCollider.transform.CompareTag("Food") ) continue;

            // Slice food
            Transform parentFood = foodCollider.transform.parent;

            // Initialize Pools
            maskPool = foodCollider.collider.GetComponent<ObjectPooler>();
            foodPool = parentFood.parent.GetComponent<ObjectPooler>();

            // Gets spawn point
            sliceEdgePoint_0 = Physics2D.Raycast(slicePoints[0], (slicePoints[1] - slicePoints[0]).normalized, 100).point;
            sliceEdgePoint_1 = Physics2D.Raycast(slicePoints[1], (slicePoints[0] - slicePoints[1]).normalized, 100).point;

            Debug.DrawLine(sliceEdgePoint_0, sliceEdgePoint_1, Color.black, 10f);
            Vector2 sliceCenter = (sliceEdgePoint_0 + sliceEdgePoint_1) / 2f;

            // Rotate mask to be parallel to slice
            bool rotateFromYAxis = (slicePoints[0].x < slicePoints[1].x && slicePoints[0].y > slicePoints[1].y) || (slicePoints[1].x < slicePoints[0].x && slicePoints[1].y > slicePoints[0].y);
            float opposite = (rotateFromYAxis) ? Mathf.Abs(slicePoints[1].x - slicePoints[0].x) : Mathf.Abs(slicePoints[1].y - slicePoints[0].y);
            float hypotenuse = Vector2.Distance(slicePoints[0], slicePoints[1]);
            float rotAng = Mathf.Asin(opposite / hypotenuse) * Mathf.Rad2Deg;

            // Get perpendicular vector
            Vector2 perpendicularSlice = Vector2.Perpendicular(slicePoints[0]-slicePoints[1]).normalized;

            // Spawn Mask
            GameObject spriteMaskObj = maskPool.GetPooledObject();
            SpriteMask spriteMask = spriteMaskObj.GetComponent<SpriteMask>();
            Vector2 spawnPos = sliceCenter + spriteMaskObj.transform.localScale.x /2f * perpendicularSlice;
            sliceDir = GetSidePointIsOn(spawnPos);
            spriteMaskObj.SetActive(true);
            spriteMaskObj.transform.SetPositionAndRotation(spawnPos, Quaternion.Euler(0,0,rotAng));
            objectsEnabledThisTurn.Add(spriteMaskObj);

            // Update polygonCollider2D
            PolygonCollider2D originalFoodCollider = (PolygonCollider2D)foodCollider.collider;
            Vector2[] originalPoints = originalFoodCollider.points;
            PolygonCollider2D newPolyFoodCollider = originalFoodCollider;
            originalPolygonColliders.Add(newPolyFoodCollider);
            List<Vector2> newColPoints = new(0);
            foreach (Vector2 point in newPolyFoodCollider.points)
            {
                Tuple<Directions, Directions> side = GetSidePointIsOn(point + (Vector2)foodCollider.transform.parent.position);
                if (side.Item1 != sliceDir.Item1 || side.Item2 != sliceDir.Item2) newColPoints.Add(point);
            }
            Vector2 pointOfIntersection = GetTwoLinesIntersectPoint(sliceEdgePoint_0, sliceEdgePoint_1, newPolyFoodCollider.points[^3],newPolyFoodCollider.points[^2]);
            Debug.Log("Poi: " + pointOfIntersection);
            bool isPointOnFood = newPolyFoodCollider.bounds.Contains(pointOfIntersection);
            Vector2 parentOffset = foodCollider.transform.parent.position; // PolyCol points are local pos
            if (newPolyFoodCollider.points[^1] == newPolyFoodCollider.points[^2] && isPointOnFood) {
                newColPoints.Add(sliceEdgePoint_0 - parentOffset);
                newColPoints.Add(pointOfIntersection - parentOffset);
                newColPoints.Add(pointOfIntersection - parentOffset);
            }
            else {
                newColPoints.Add(sliceEdgePoint_0 - parentOffset);
                newColPoints.Add(sliceEdgePoint_1 - parentOffset);
                newColPoints.Add(sliceEdgePoint_1 - parentOffset);
            }
            newPolyFoodCollider.SetPath(0,newColPoints);
           

            // Create other side slice
            GameObject otherSlice = foodPool.GetPooledObject();
            otherSlice.transform.SetPositionAndRotation(parentFood.position,parentFood.rotation);
            
            // Set up other slice sprite masks
            float separationSpace = 0.05f;
            int maskIndex = 0;
            ObjectPooler otherSliceMaskPool = otherSlice.transform.GetChild(0).GetComponent<ObjectPooler>();
            GameObject currentMask = maskPool.GetPooledObjectAtIndex(maskIndex);

            // Copy all sprite masks of this slice to other slice
            int numMasks = maskPool.GetNumObjectsActive();
            while (maskIndex < numMasks-1) {
                GameObject prevSliceMask = otherSliceMaskPool.GetPooledObject();
                prevSliceMask.SetActive(true);
                prevSliceMask.transform.SetPositionAndRotation(currentMask.transform.position, currentMask.transform.rotation);
                objectsEnabledThisTurn.Add(prevSliceMask);

                maskIndex++;
                currentMask = maskPool.GetPooledObjectAtIndex(maskIndex);
            }

            // Update other slice polygonCollider2D
            PolygonCollider2D newSlicePolyFoodCollider = otherSlice.GetComponentInChildren<PolygonCollider2D>();
            originalPolygonColliders.Add(newSlicePolyFoodCollider);
            if (!newSlicePolyFoodCollider) {
                throw new System.Exception("Slice doesn't have polygonCollider2D");
            }
            List<Vector2> newSliceColPoints = new(0);
            foreach (Vector2 point in originalPoints)
            {
                Tuple<Directions, Directions> side = GetSidePointIsOn(point + (Vector2)foodCollider.transform.parent.position);
                if (side.Item1 == sliceDir.Item1 || side.Item2 == sliceDir.Item2) newSliceColPoints.Add(point);
            }
            pointOfIntersection = GetTwoLinesIntersectPoint(sliceEdgePoint_0, sliceEdgePoint_1, newPolyFoodCollider.points[^3],newPolyFoodCollider.points[^2]);
            isPointOnFood = newPolyFoodCollider.bounds.Contains(pointOfIntersection);
            parentOffset  = -foodCollider.transform.parent.position;
            if (newPolyFoodCollider.points[^1] == newPolyFoodCollider.points[^2] && isPointOnFood) {
                newSliceColPoints.Add(sliceEdgePoint_1 + parentOffset);
                newSliceColPoints.Add(pointOfIntersection + parentOffset);
                newSliceColPoints.Add(pointOfIntersection + parentOffset);
                // newColPoints.Add(sliceEdgePoint_0);
            }
            else {
                
                newSliceColPoints.Add(sliceEdgePoint_1 + parentOffset);
                newSliceColPoints.Add(sliceEdgePoint_0 + parentOffset);
                newSliceColPoints.Add(sliceEdgePoint_0 + parentOffset);
            }
            newSlicePolyFoodCollider.SetPath(0,newSliceColPoints);

            GameObject finalSliceMask = otherSliceMaskPool.GetPooledObject();
            finalSliceMask.SetActive(true);
            finalSliceMask.transform.SetPositionAndRotation(sliceCenter - spriteMaskObj.transform.localScale.x / 2f * perpendicularSlice,Quaternion.Euler(0,0,rotAng));
            objectsEnabledThisTurn.Add(finalSliceMask);

            // Separate Slices
            parentFood.Translate(-perpendicularSlice * separationSpace);
            otherSlice.transform.Translate(perpendicularSlice * separationSpace);
            otherSlice.SetActive(true);

            // Add to undo stack
            objectsEnabledThisTurn.Add(otherSlice);
            AudioManager.Instance.PlaySFX("Slice");
        }
        everyMoveGameObject.Push(objectsEnabledThisTurn);
        everyMovePolygonColliders.Push(originalPolygonColliders);
        ResetSlicePoints();
        UpdateCurrentSlicesCount(-1);
    }

    public void HoldKnife() {
        IsHoldingKnife = true;
        ResetSlicePoints();
    }

    public void DropKnife() {
        IsHoldingKnife = false;
        ResetSlicePoints();
    }

    public void UndoSlice() {
        if (currentUndoIndex <= 0) return;
        List<PolygonCollider2D> pcList = everyMovePolygonColliders.Pop();
        int i = 0;
        foreach (GameObject obj in everyMoveGameObject.Pop())
        {
            // obj = specific sprite mask | obj parent = Sprite Masks | obj parent parent = food
            obj.transform.parent.parent.GetComponentInChildren<PolygonCollider2D>().points = pcList[i].points;
            obj.SetActive(false);
        }
        currentUndoIndex--;
        UpdateCurrentSlicesCount(1);
        // disable spritemasks
        // Disable food
    }

    // This function may break if lossy scale != 1
    private Tuple<Directions, Directions> GetSidePointIsOn(Vector2 point) {
        float slope = (sliceEdgePoint_1.y - sliceEdgePoint_0.y) / (sliceEdgePoint_1.x - sliceEdgePoint_0.x);
        float yLine = GetSlopeIntercept(sliceEdgePoint_0, sliceEdgePoint_1, point.x);
        float yDelta = yLine - point.y;
        Directions xDir = Directions.on;
        Directions yDir = Directions.on;
        if (yDelta != 0) yDir = (yDelta > 0 ) ? Directions.below : Directions.above;
        if (slope != 0) xDir = ((slope > 0 && yDir == Directions.above) || (slope < 0 && yDir == Directions.below)) ? Directions.right :Directions.left; 
        return new(xDir, yDir);
    }

    private float GetSlopeIntercept(Vector2 a, Vector2 b, float x) {
        float slope = (a.y - b.y) / (a.x - b.x);
        float yIntercept = a.y - slope * a.x;
        return slope * x + yIntercept;
    }

    private Vector2 GetTwoLinesIntersectPoint(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2) {
        float slope1 = (a1.y - b1.y) / (a1.x - b1.x);
        float yIntercept1 = a1.y - slope1 * a1.x;
        float slope2 = (a2.y - b2.y) / (a2.x - b2.x);
        float yIntercept2 = a2.y - slope2 * a2.x;
        float x = (yIntercept2 - yIntercept1) / (slope1 - slope2);
        float y = slope1 * x + yIntercept1;

        if (slope1 == slope2) return Vector2.positiveInfinity;
        return new(x, y);
    }

    private void UpdateCurrentSlicesCount(int val) {
        currentSlicesLeft += val;
        currentSlicesLeft = math.clamp(currentSlicesLeft, 0, maxSlicesCount);
        UpdateSlicesText();
    }
    private void UpdateSlicesText() {
        slicesText.text = "Slices left: " + currentSlicesLeft;
    }
}
