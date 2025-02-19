using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class BoxMenuControls : MonoBehaviour
{
    public float shiftValue;
    [Tooltip("How long in secs shifting takes")]
    [SerializeField] float shiftDuration = 1f;

    RectTransform boxHolder;
    int numBoxes = 0;
    float maxXVal = 0;

    void Awake()
    {
        boxHolder = GetComponent<RectTransform>();
    }
    void Start()
    {
        numBoxes = boxHolder.childCount;
        maxXVal = -(numBoxes - 1) * shiftValue;
    }

    public void GoLeft() {
        StartCoroutine(ShiftBox(true));
    }

    public void GoRight() {
        StartCoroutine(ShiftBox(false));
    }

    // true = left; false = right
    private IEnumerator ShiftBox(bool dir) {
        float xGoal = boxHolder.anchoredPosition.x + (dir ? shiftValue : -shiftValue);
        xGoal = Mathf.Clamp(xGoal, maxXVal, 0f);
        float rate = ((dir ? shiftValue : -shiftValue) / shiftDuration) * Time.deltaTime;
        Vector2 newPos = boxHolder.anchoredPosition;
        if (dir) {
            while(boxHolder.anchoredPosition.x < xGoal)
            {
                newPos.x += rate;
                boxHolder.anchoredPosition = newPos;
                yield return null;
            }
            newPos.x = xGoal;
            boxHolder.anchoredPosition = newPos;
        } else {
            while(boxHolder.anchoredPosition.x > xGoal)
            {
                newPos.x += rate;
                boxHolder.anchoredPosition = newPos;
                yield return null;
            }
            newPos.x = xGoal;
            boxHolder.anchoredPosition = newPos;
        }
    }
}
