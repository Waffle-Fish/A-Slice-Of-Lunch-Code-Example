using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    [SerializeField]
    GameObject foodsObj;
    List<ControlFood> totalFoodList = new();
    List<ControlFood> currentFoodList = new();
    private Collider2D container;

    void Awake()
    {
        container = GetComponent<Collider2D>();
    }

    void Start()
    {
        UpdateTotalFoodList();
    }

    private void UpdateTotalFoodList() {
        totalFoodList.Clear();
        foodsObj.GetComponentsInChildren<ControlFood>(totalFoodList);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // if (!IsFoodInBox(col)) return;
        Debug.Log(col.name + " is in the box");
        // if (currentFoodList.)
    }

    bool IsFoodInBox(Collider2D col)
    {
        //Debug.Log("bounds: "+container.bounds);
        PolygonCollider2D col_p = (PolygonCollider2D) col;
        foreach (Vector2 point in col_p.points)
        {
            Vector3 col_global_point = col.transform.parent.position + (Vector3) point /transform.lossyScale.x;
            //Debug.Log(point);
            if (!container.bounds.Contains(col_global_point))
            {
                return false;
            }
        }
        return true;
    }
}
