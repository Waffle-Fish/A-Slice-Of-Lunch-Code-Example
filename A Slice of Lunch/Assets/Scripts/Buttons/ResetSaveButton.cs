using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetSaveButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SaveSystem.Instance == null) Debug.LogError("No Save System in this level!");
        else SaveSystem.Instance.ResetLevels();
    }
}
