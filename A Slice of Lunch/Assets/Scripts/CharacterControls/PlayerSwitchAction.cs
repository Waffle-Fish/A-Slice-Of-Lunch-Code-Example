using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerActions { move, slice, rotate }

[RequireComponent(typeof(PlayerMoveFood), typeof(PlayerSlice), typeof(PlayerRotate))]
public class PlayerSwitchAction : MonoBehaviour
{
    PlayerActions currentAction = PlayerActions.move;
    PlayerMoveFood moveScript;
    PlayerSlice sliceScript;
    PlayerRotate rotateScript;

    int ActionsLength;
    private PlayerInputActions.PlayerActions playerActions;

    public static event Action<PlayerActions> OnPlayerActionChange;

    void Start()
    {
        moveScript = GetComponent<PlayerMoveFood>();
        sliceScript = GetComponent<PlayerSlice>();
        rotateScript = GetComponent<PlayerRotate>();

        DisableScripts();
        moveScript.enabled = true;

        ActionsLength = Enum.GetValues(typeof(PlayerActions)).Length;
        playerActions = PlayerInputManager.Instance.PlayerActions;
    }
    
    private void OnEnable() {
        ToolButton.OnPlayerActionChange += SwitchPlayerActions;
    }

    private void OnDisable() {
        ToolButton.OnPlayerActionChange -= SwitchPlayerActions;
    }

    private void Update()
    {
        DetectRightClick();
    }

    private void DetectRightClick()
    {
        if (playerActions.RightClick.WasPressedThisFrame()) SwitchPlayerActions();
    }

    private void SwitchPlayerActions()
    {
        currentAction = (PlayerActions)((int)(currentAction + 1) % ActionsLength);
        SwitchPlayerActions(currentAction);
        
    }

    private void SwitchPlayerActions(PlayerActions playerAction)
    {
        currentAction = playerAction;
        DisableScripts();
        switch (currentAction)
        {
            case PlayerActions.move:
                moveScript.enabled = true;
                break;
            case PlayerActions.slice:
                sliceScript.enabled = true;
                break;
            case PlayerActions.rotate:
                rotateScript.enabled = true;
                break;
        }
        OnPlayerActionChange?.Invoke(currentAction);
        // Debug.Log("Player Action switched to: " + currentAction);
    }

    private void DisableScripts()
    {
        moveScript.enabled = false;
        sliceScript.enabled = false;
        rotateScript.enabled = false;
    }
}
