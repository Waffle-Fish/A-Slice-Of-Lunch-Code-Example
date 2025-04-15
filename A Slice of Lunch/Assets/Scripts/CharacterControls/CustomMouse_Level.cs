using UnityEngine;
using UnityEngine.InputSystem;

public class CustomMouse_Level : MonoBehaviour {
    [SerializeField] Sprite finger;
    [SerializeField] Sprite fist;
    [SerializeField] Sprite knife;
    [SerializeField] Vector3 offset = Vector3.zero;
    SpriteRenderer spriteRenderer;

    private void Start() {
        Cursor.visible = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = finger;
    }

    private void Update() {
        if (Mouse.current.rightButton.wasPressedThisFrame) {spriteRenderer.sprite = knife;}
        else if (Mouse.current.rightButton.isPressed) {spriteRenderer.sprite = knife;}
        else if (Mouse.current.leftButton.isPressed) {spriteRenderer.sprite = fist;}
        else { spriteRenderer.sprite = finger; }
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + ((spriteRenderer.sprite == knife) ? Vector3.zero : offset);
    }


}
