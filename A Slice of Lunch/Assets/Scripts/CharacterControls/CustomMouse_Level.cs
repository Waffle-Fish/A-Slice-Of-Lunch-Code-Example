using UnityEngine;
using UnityEngine.InputSystem;

public class CustomMouse_Level : MonoBehaviour {
    [SerializeField] Sprite finger;
    [SerializeField] Sprite fist;
    [SerializeField] Sprite knife;
    [SerializeField] Vector3 offset = Vector3.zero;
    SpriteRenderer spriteRenderer;

    enum HandState {Point, Grab, Knife}
    HandState currentHand = HandState.Point;

    private PlayerInputActions inputActions;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputActions = new();
    }
    private void Start() {
        Cursor.visible = false;
        spriteRenderer.sprite = finger;
    }

    private void OnEnable() {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }


    private void Update() {
        // if (Mouse.current.rightButton.wasPressedThisFrame) {spriteRenderer.sprite = knife;}
        // else if (Mouse.current.rightButton.isPressed) {spriteRenderer.sprite = knife;}
        // else if (Mouse.current.leftButton.isPressed) {spriteRenderer.sprite = fist;}
        // else { spriteRenderer.sprite = finger; }
        // transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + ((spriteRenderer.sprite == knife) ? Vector3.zero : offset);
        Collider2D overlapCol = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) + ((spriteRenderer.sprite == knife) ? Vector3.zero : offset));
        if (inputActions.Player.LeftClick.WasPressedThisFrame()) {
            if(overlapCol && overlapCol.CompareTag("Food")) { currentHand = HandState.Grab; }
            else { currentHand = HandState.Knife; }
        } 
        if (inputActions.Player.LeftClick.WasReleasedThisFrame()) { currentHand = HandState.Point; }

        spriteRenderer.sprite = currentHand switch
        {
            HandState.Grab => fist,
            HandState.Knife => knife,
            _ => finger,
        };

        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + ((spriteRenderer.sprite == knife) ? Vector3.zero : offset);
    }
}
