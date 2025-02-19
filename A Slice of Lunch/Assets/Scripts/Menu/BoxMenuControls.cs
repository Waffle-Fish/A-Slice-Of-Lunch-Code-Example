using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMenuControls : MonoBehaviour
{
    public float shiftValue;
    RectTransform boxHolder;
    int numBoxes = 0;
    float maxXVal = 0;
    // Start is called before the first frame update

    void Awake()
    {
        boxHolder = GetComponent<RectTransform>();
    }
    void Start()
    {
        numBoxes = boxHolder.childCount;
        maxXVal = -(numBoxes - 1) * shiftValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GoLeft() {
        ShiftBox(true);
    }

    public void GoRight() {
        ShiftBox(false);
    }

    // true = left; false = right
    private void ShiftBox(bool dir) {
        Vector2 newPos = boxHolder.anchoredPosition;
        newPos.x = Mathf.Clamp((dir) ? newPos.x + shiftValue : newPos.x - shiftValue, maxXVal ,0f);
        boxHolder.anchoredPosition = newPos;
    }
}
