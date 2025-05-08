using UnityEngine;
using UnityEngine.InputSystem;

public class CustomMouse_Level : MonoBehaviour {
    [SerializeField] Sprite finger;
    [SerializeField] Sprite fist;
    [SerializeField] Sprite knife;
    [SerializeField] Vector3 offset = Vector3.zero;
    SpriteRenderer spriteRenderer;
    public enum HandState {Point, Grab, Knife}

    HandState currentHand;

    private PlayerInputActions.PlayerActions playerActions;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        // playerActions.LeftClick.performed += Grab;    
    }

    private void Start() {
        Cursor.visible = false;
        spriteRenderer.sprite = finger;
        playerActions = PlayerInputManager.Instance.PlayerActions;
    }

    private void Update() {
        if (Mouse.current.rightButton.wasPressedThisFrame) {spriteRenderer.sprite = knife;}
        else if (Mouse.current.rightButton.isPressed) {spriteRenderer.sprite = knife;}
        else if (Mouse.current.leftButton.isPressed) {spriteRenderer.sprite = fist;}
        else { spriteRenderer.sprite = finger; }
        // transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + ((spriteRenderer.sprite == knife) ? Vector3.zero : offset);
        Vector2 mousePos = playerActions.MousePosition.ReadValue<Vector2>();
        Collider2D overlapCol = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(mousePos) + ((spriteRenderer.sprite == knife) ? Vector3.zero : offset));
     

        transform.position = Camera.main.ScreenToWorldPoint(mousePos) + ((spriteRenderer.sprite == knife) ? Vector3.zero : offset);
    }


}
