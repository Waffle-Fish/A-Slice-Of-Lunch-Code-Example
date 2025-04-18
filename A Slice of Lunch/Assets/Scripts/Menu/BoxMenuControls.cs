using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxMenuControls : MonoBehaviour
{
    public float shiftValue;
    [Tooltip("How long in secs shifting takes")]
    [SerializeField] float shiftDuration = 1f;
    [SerializeField] Animator leftButton;
    [SerializeField] Animator rightButton;

    RectTransform boxHolder;
    int numBoxes = 0;
    float maxXVal = 0;
    int currentBox = 0;
    public bool IsActive {get; private set;} = false;

    List<Animator> animators = new();

    void Awake()
    {
        boxHolder = GetComponent<RectTransform>();
        GetComponentsInChildren<Animator>(animators);
    }
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf) numBoxes++;
        }
        maxXVal = -(numBoxes - 1) * shiftValue;
    }

    public void GoLeft() {
        if(IsActive) return;
        IsActive = true;
        CloseAllNotePads();
        StartCoroutine(ShiftBox(true));
    }

    public void GoRight() {
        if(IsActive) return;
        IsActive = true;
        CloseAllNotePads();
        StartCoroutine(ShiftBox(false));
        
    }

    // true = left; false = right
    private IEnumerator ShiftBox(bool goLeft) {
        
        if (currentBox != 0) leftButton.SetTrigger("FadeOut");
        if (currentBox != numBoxes -1) rightButton.SetTrigger("FadeOut");

        float xGoal = boxHolder.anchoredPosition.x + (goLeft ? shiftValue : -shiftValue);
        xGoal = Mathf.Clamp(xGoal, maxXVal, 0f);
        float rate = ((goLeft ? shiftValue : -shiftValue) / shiftDuration) * Time.deltaTime;
        Vector2 newPos = boxHolder.anchoredPosition;
        if (goLeft) {
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
        currentBox += (goLeft) ? -1 : 1;
        currentBox = Mathf.Clamp(currentBox, 0, numBoxes-1);

        if (!leftButton.gameObject.activeSelf) {leftButton.gameObject.SetActive(true);}
        if (!rightButton.gameObject.activeSelf) {rightButton.gameObject.SetActive(true);}
        if (currentBox != 0) leftButton.SetTrigger("FadeIn");
        if (currentBox != numBoxes -1) rightButton.SetTrigger("FadeIn");
        

        IsActive = false;
    }

    private void CloseAllNotePads() {
        foreach (var anim in animators)
        {
            anim.ResetTrigger("Open");
            
            anim.SetTrigger("Close");
        }
    }
}
