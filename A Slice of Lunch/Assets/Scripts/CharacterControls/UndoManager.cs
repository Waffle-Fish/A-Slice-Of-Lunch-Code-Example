using UnityEngine;
using System.Collections.Generic;

public struct SliceDate {
    public GameObject spriteMaskObj;
    public Vector2[] originalPolyColPoints;
    public float originalSliceZRot;
}

public struct PreviousTurns {
    public List<SliceDate> slicesModifiedThisTurn;
}

public class UndoManager : MonoBehaviour
{
    Stack<PreviousTurns> everyTurnsMade;

    PlayerSlice playerSlice;

    private void Awake() {
        playerSlice = GetComponent<PlayerSlice>();
    }

    public void UndoSlice() {
        if (everyTurnsMade.Count <= 0) return;
        PreviousTurns TurnToUndo = everyTurnsMade.Pop();
        foreach (SliceDate food in TurnToUndo.slicesModifiedThisTurn)
        {
            // obj = specific sprite mask | obj parent = Sprite Masks | obj parent parent = food
            food.spriteMaskObj.transform.parent.parent.GetComponentInChildren<PolygonCollider2D>().points = food.originalPolyColPoints;
            food.spriteMaskObj.SetActive(false);
        }
        playerSlice.UpdateCurrentSlicesCount(1);
        WinManager.Instance.UpdateTotalFoodList();
    }
}
