using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestingRaycast : MonoBehaviour
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
            endPoints[i].layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        ResetSlicePoints();
        currentSlicesLeft = maxSlicesCount;
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
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(clickRay);
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

        // Gets spawn point
        foreach (var foodCollider in slicedObjects)
        {
            int originalLayer = foodCollider.collider.gameObject.layer;
            int sliceLayer = LayerMask.NameToLayer("Slice");
            foodCollider.collider.gameObject.layer = sliceLayer;
            // These points are in world coordinates, not local
            RaycastHit2D hit0 = Physics2D.Raycast(slicePoints[0], (slicePoints[1] - slicePoints[0]), 100f, sliceLayer);
            RaycastHit2D hit1 = Physics2D.Raycast(slicePoints[1], (slicePoints[0] - slicePoints[1]), 100f, sliceLayer);
            sliceEdgePoint_0 = hit0.point;
            sliceEdgePoint_1 = hit1.point;
            Debug.DrawLine(hit0.point, hit1.point, Color.red, 10f);
            Debug.Log("Slice Edge 0: " + sliceEdgePoint_0 + " Slice Edge 1: " + sliceEdgePoint_1);
            foodCollider.collider.gameObject.layer = originalLayer; 
        }
        
        ResetSlicePoints();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
    }
}
