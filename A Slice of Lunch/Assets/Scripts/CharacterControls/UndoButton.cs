using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UndoButton : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnUndoButtonPressed;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) OnUndoButtonPressed?.Invoke();
    }
}
