using UnityEngine;
using System.Collections.Generic;
using System;

public struct SliceObjectData {
    // public GameObject foodObject;
    public GameObject spriteMaskObj;
    public PolygonCollider2D polygonCollider2D;
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
    private Animator anim;

    private void OnEnable()
    {
        PlayerSlice.OnSliceFinish += UpdateTurns;
        UndoButton.OnUndoButtonPressed += UndoSlice;
    }

    private void OnDisable()
    {
        PlayerSlice.OnSliceFinish -= UpdateTurns;
        UndoButton.OnUndoButtonPressed -= UndoSlice;
    }

    private void UpdateTurns(TurnActions currentTurn)
    {
        everyTurnsMade.Push(currentTurn);
    }

    private void Awake()
    {
        playerSlice = GetComponent<PlayerSlice>();
        anim = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
    }

    public void UndoSlice()
    {
        // anim.SetTrigger("PoofTrigger");

        if (everyTurnsMade.Count <= 0) return;
        TurnActions TurnToUndo = everyTurnsMade.Pop();
        foreach (SliceObjectData food in TurnToUndo.slicesModifiedThisTurn)
        {
            // obj = specific spritemask | obj parent = Sprite Masks | obj parent parent = food
            food.spriteMaskObj.transform.parent.parent.GetComponentInChildren<PolygonCollider2D>().points = food.originalPolyColPoints;
            food.spriteMaskObj.SetActive(false);
        }
        foreach (GameObject foodObj in TurnToUndo.foodsToDisableThisTurn)
        {
            // On Disable food object, disable every spritemasks
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

    public void PlayAnimation()
    {
        
    }


}
