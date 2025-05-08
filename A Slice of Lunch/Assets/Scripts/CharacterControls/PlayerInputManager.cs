using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }
    public PlayerInputActions InputActions { get; private set; }
    public PlayerInputActions.PlayerActions PlayerActions { get; private set; }
    public Vector2 MousePos { get; private set; } = Vector2.zero;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this; 

        InputActions = new();
        PlayerActions = InputActions.Player;
    }

    private void OnEnable() {
        InputActions.Enable();
    }

    private void OnDisable() {
        InputActions.Disable();
    }

    private void Update() {
        MousePos = PlayerActions.MousePosition.ReadValue<Vector2>();
    }
}
