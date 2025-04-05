using UnityEngine;
using UnityEngine.UI;

public class CustomShapedButton : MonoBehaviour
{
    private Image buttonImg;

    void Awake() {
        buttonImg = GetComponent<Image>();
    }

    void Start()
    {
        buttonImg.alphaHitTestMinimumThreshold = 0.5f;
    }
}
