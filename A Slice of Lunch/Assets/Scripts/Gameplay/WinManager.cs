using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    public static WinManager Instance { get; private set; }
    [SerializeField]
    GameObject foodsObj;
    [SerializeField]
    TextMeshProUGUI piecesLeftText;
    [SerializeField]
    GameObject winButton;
    // List<ControlFood> totalFoodList = new();
    // List<ControlFood> foodInBox = new();
    List<PolygonCollider2D> totalFoodList = new();
    List<PolygonCollider2D> foodInBox = new();
    private Collider2D container;
    private bool foodIsMoving = false;
    private bool foodIsRotating = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this; 
        container = GetComponent<Collider2D>();
    }

    void Start()
    {
        UpdateTotalFoodList();
    }

    private void OnEnable()
    {
        PlayerMoveFood.OnFoodIsMoving += UpdateFoodIsMoving;
        PlayerRotate.OnFoodIsRotating += UpdateFoodIsRotating;
    }

    private void OnDisable()
    {
        PlayerMoveFood.OnFoodIsMoving -= UpdateFoodIsMoving;
        PlayerRotate.OnFoodIsRotating -= UpdateFoodIsRotating;
    }

    void Update() {
        bool win = foodInBox.Count == totalFoodList.Count && !foodIsMoving && !foodIsRotating;
        winButton.SetActive(win);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // Debug.Log(col.name + " is in the box");
        PolygonCollider2D foodCol = col.GetComponent<PolygonCollider2D>();
        if (!foodInBox.Contains(foodCol)) {
            foodInBox.Add(foodCol);
            UpdatePiecestOutsideBox();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        foodInBox.Remove(other.GetComponent<PolygonCollider2D>());
    }

    void UpdateFoodIsMoving(bool b)
    {
        foodIsMoving = b;
    }

    void UpdateFoodIsRotating(bool b)
    {
        foodIsRotating = b;
    }

    public void UpdateTotalFoodList()
    {
        totalFoodList.Clear();
        // foodsObj.GetComponentsInChildren<ControlFood>(false,totalFoodList);
        foodsObj.GetComponentsInChildren<PolygonCollider2D>(false, totalFoodList);
        // UpdatePiecestOutsideBox();
    }

    private void UpdatePiecestOutsideBox() {
        // piecesLeftText.text = $"Pieces outside lunchbox: {totalFoodList.Count - foodInBox.Count}";
    }
}
