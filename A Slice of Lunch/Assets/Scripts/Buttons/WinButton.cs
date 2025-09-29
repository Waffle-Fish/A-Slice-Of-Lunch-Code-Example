using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinButton : MonoBehaviour, IPointerClickHandler
{
    SpriteRenderer worldMouseSprite;
    PlayerSwitchAction playerSwitchAction;
    void Start()
    {
        playerSwitchAction = PlayerInputManager.Instance.gameObject.GetComponent<PlayerSwitchAction>();
        if (!playerSwitchAction) Debug.LogError("Player Input Manager isn't in the same game object as player switch action");
        worldMouseSprite = playerSwitchAction.GetComponentInChildren<SpriteRenderer>();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        playerSwitchAction.DisableAllActions();
        worldMouseSprite.enabled = false;
    }
}
