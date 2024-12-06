using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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
    List<ControlFood> totalFoodList = new();
    List<ControlFood> foodInBox = new();
    private Collider2D container;

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

    void Update() {
        winButton.SetActive(foodInBox.Count == totalFoodList.Count);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // if (!IsFoodInBox(col)) { return; }
        ControlFood cf = col.GetComponent<ControlFood>();
        if (!foodInBox.Contains(cf)) foodInBox.Add(cf);
        UpdatePiecestOutsideBox();
        // Debug.Log(col.transform.parent.name + " is in the box");
        
    }

    private void OnTriggerExit2D(Collider2D other) {
        foodInBox.Remove(other.GetComponent<ControlFood>());
        UpdatePiecestOutsideBox();
        // Debug.Log(other.transform.parent.name + " has left the box");
    }

    bool IsFoodInBox(Collider2D col)
    {
        // Debug.Log("bounds: "+container.bounds);
        PolygonCollider2D col_p = (PolygonCollider2D) col;
        foreach (Vector2 point in col_p.points)
        {
            Vector3 col_global_point = col.transform.parent.position + (Vector3) point / transform.lossyScale.x;
            //Debug.Log(point);
            if (!container.bounds.Contains(col_global_point))
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateTotalFoodList() {
        totalFoodList.Clear();
        foodsObj.GetComponentsInChildren<ControlFood>(false,totalFoodList);
        UpdatePiecestOutsideBox();
        
    }

    private void UpdatePiecestOutsideBox() {
        piecesLeftText.text = $"Pieces outside lunchbox: {totalFoodList.Count - foodInBox.Count}";
    }
}
