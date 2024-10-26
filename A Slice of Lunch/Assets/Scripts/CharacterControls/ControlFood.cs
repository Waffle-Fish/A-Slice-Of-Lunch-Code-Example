using UnityEngine;

public class ControlFood : MonoBehaviour
{
    bool dragging = false;
    PlayerControls playerControls;
    
    private void Awake() {
        playerControls = GameObject.FindWithTag("Player").GetComponent<PlayerControls>();
    }

    private void OnMouseDown() {
        if (playerControls.IsHoldingKnife) return;
        // Vector3 camWorldPos = Camera.main.transform.position;
        // Collider2D[] overlaps = Physics2D.OverlapPointAll(camWorldPos);
        // foreach (var col in overlaps)
        // {
        //     if (col.GetComponent<SpriteMask>()) return;
        // }
        dragging = true;
    }

    private void Update() {
        if (dragging) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            transform.parent.position = pos;
        }
    }

    private void OnMouseUp() {
        dragging = false;
    }
}
