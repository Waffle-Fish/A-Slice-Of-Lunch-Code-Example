using UnityEngine;
using UnityEngine.UI;

public class CustomShapedButton : MonoBehaviour
{
    [Tooltip("Minimum alpha a pixel must have for the event to considered a hit on the Image")]
    [Range(0,1f)]
    [SerializeField] float alphaHitTestMinimumThreshold = 0.5f;
    private Image buttonImg;

    void Awake() {
        buttonImg = GetComponent<Image>();
    }

    void Start()
    {
        buttonImg.alphaHitTestMinimumThreshold = alphaHitTestMinimumThreshold;
    }
}
