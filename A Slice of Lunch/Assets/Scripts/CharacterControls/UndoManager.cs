using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using System.Collections;

public struct SliceObjectData {
    // public GameObject foodObject;
    public GameObject SpriteMaskObj;
    public PolygonCollider2D PolyCol2D;
    public Vector2[] OriginalPolyColPoints;
    public Vector3 LossyScale;
    public float OriginalSliceZRot;
}

public struct TurnActions
{
    public List<SliceObjectData> slicesModifiedThisTurn;
    public List<GameObject> foodsToDisableThisTurn;
    public List<Tuple<Transform,Vector2>> foodPositionsThisTurn;
}

public class UndoManager : MonoBehaviour
{
    const float VFX_DELAY = 0.2f;
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
        IEnumerator DelayUndo()
        {
            yield return new WaitForSeconds(VFX_DELAY);
            ProcessUndo();
        }

        void ProcessUndo()
        {
            if (everyTurnsMade.Count <= 0) return;
            TurnActions TurnToUndo = everyTurnsMade.Pop();
            foreach (SliceObjectData food in TurnToUndo.slicesModifiedThisTurn)
            {
                // obj = specific spritemask | obj parent = Sprite Masks | obj parent parent = food
                food.SpriteMaskObj.transform.parent.parent.GetComponentInChildren<PolygonCollider2D>().points = food.OriginalPolyColPoints;
                food.SpriteMaskObj.SetActive(false);
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
            foreach (var food in TurnToUndo.foodPositionsThisTurn)
            {
                food.Item1.position = food.Item2;
            }
            playerSlice.UpdateCurrentSlicesCount(1);
            WinManager.Instance.UpdateTotalFoodList();
        }

        StartCoroutine(DelayUndo());
    }
}
