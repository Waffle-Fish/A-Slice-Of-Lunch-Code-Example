using UnityEngine;
using System.Collections.Generic;
using System;

public struct SliceObjectData {
    // public GameObject foodObject;
    public GameObject spriteMaskObj;
    public Vector2[] originalPolyColPoints;
    public float originalSliceZRot;
}

public struct TurnActions {
    public List<SliceObjectData> slicesModifiedThisTurn;
    public List<GameObject> foodsToDisableThisTurn;
}

public class UndoManager : MonoBehaviour
{
    Stack<TurnActions> everyTurnsMade = new();

    PlayerSlice playerSlice;

    private void OnEnable() {
        PlayerSlice.OnSliceFinish += UpdateTurns;
    }

    private void OnDisable() {    
        PlayerSlice.OnSliceFinish -= UpdateTurns;
    }

    private void UpdateTurns(TurnActions currentTurn)
    {
        everyTurnsMade.Push(currentTurn);
    }

    private void Awake() {
        playerSlice = GetComponent<PlayerSlice>();
    }

    public void UndoSlice() {
        if (everyTurnsMade.Count <= 0) return;
        TurnActions TurnToUndo = everyTurnsMade.Pop();
        foreach (SliceObjectData food in TurnToUndo.slicesModifiedThisTurn)
        {
            // obj = specific sprite mask | obj parent = Sprite Masks | obj parent parent = food
            food.spriteMaskObj.transform.parent.parent.GetComponentInChildren<PolygonCollider2D>().points = food.originalPolyColPoints;
            food.spriteMaskObj.SetActive(false);
        }
        foreach (GameObject foodObj in TurnToUndo.foodsToDisableThisTurn)
        {
            // Get spriteMasks in foodObj
            SpriteMask[] foodSpriteMasks = foodObj.GetComponentsInChildren<SpriteMask>();
            foreach (var spriteMask in foodSpriteMasks)
            {
                spriteMask.gameObject.SetActive(false);
            }
            foodObj.SetActive(false);
        }
        playerSlice.UpdateCurrentSlicesCount(1);
        WinManager.Instance.UpdateTotalFoodList();
    }
}
