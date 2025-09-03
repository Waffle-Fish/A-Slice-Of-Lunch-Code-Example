using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.U2D.IK;

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
    private bool foodIsDragging = false;

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
    
    private void OnEnable() {
        PlayerMoveFood.OnFoodIsDragging += UpdateFoodIsDragging;
    }

    private void OnDisable() {
        PlayerMoveFood.OnFoodIsDragging -= UpdateFoodIsDragging;
    }

    void Update() {
        bool win = foodInBox.Count == totalFoodList.Count && !foodIsDragging;
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
        // UpdatePiecestOutsideBox();
        // Debug.Log(other.transform.parent.name + " has left the box");
    }

    // void IsFoodInBox(Collider2D foodCol)
    // {
    //     ContactFilter2D cf2d = new();
    //     List<Collider2D> results = new();
    //     if (Physics2D.OverlapCollider(container,cf2d.NoFilter(), results) == 0) return;
    //     if (results.Contains(foodCol) && !foodInBox.Contains((PolygonCollider2D)foodCol))
    //     {
            
    //     }
    //         foodInBox.Add((PolygonCollider2D)foodCol);
    // }

    void UpdateFoodIsDragging(bool b)
    {
        foodIsDragging = b;
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
