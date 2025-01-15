using UnityEngine;
using UnityEngine.Rendering;

public class ControlFood : MonoBehaviour
{
    bool dragging = false;
    PlayerControls playerControls;
    SortingGroup sortingGroup;
    int initialSortingOrder;
    
    private void Awake() {
        playerControls = GameObject.FindWithTag("Player").GetComponent<PlayerControls>();
        sortingGroup = GetComponentInParent<SortingGroup>();
        initialSortingOrder = sortingGroup.sortingOrder;
    }

    private void OnMouseDown() {
        if (playerControls.IsHoldingKnife) return;
        // TODO:
        // Center food slice to center of mouse, or to where it clicked
        dragging = true;
    }

    private void Update() {
        if (dragging) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            transform.parent.position = pos;
            sortingGroup.sortingOrder = 100;
        }
        else {
            sortingGroup.sortingOrder = initialSortingOrder;
        }
    }

    private void OnMouseUp() {
        dragging = false;
    }
}
