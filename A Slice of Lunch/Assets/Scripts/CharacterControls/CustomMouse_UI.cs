using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class CustomMouse_UI : MonoBehaviour
{
    RectTransform rectTransform;
    Image image;

    private void Start() {
        Cursor.visible = false;
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Update() {
        rectTransform.position = Input.mousePosition;
    }
}
