using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolButton : MonoBehaviour, IPointerClickHandler
{
    public static event Action<PlayerActions> OnPlayerActionChange;

    [SerializeField] PlayerActions tool;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPlayerActionChange?.Invoke(tool);
    }
}
