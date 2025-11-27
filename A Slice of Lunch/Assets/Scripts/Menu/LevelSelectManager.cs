using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
struct LevelBox
{
    public RectTransform LevelContainer;
    public CinemachineVirtualCamera VirtualCam;
}

public class LevelSelectManager : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField] List<LevelBox> LevelBoxes;
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;
    int currentBoxInd = 0;

    [Header("Animation Settings")]
    [SerializeField][Min(0.00001f)] float fadeDuration = 0.5f;
    [SerializeField] Vector2 boxYRange = new(-375, 125);
    [SerializeField][Min(0.000001f)] float pushPopDuration = 0.1f;

    private void Start()
    {
        CheckSerializeFieldVariables();
        currentBoxInd = 1;
        DisableAllCamerasExceptOne(currentBoxInd);
        Color transparent = new(1, 1, 1, 0);
        if (currentBoxInd == 0)
        {
            leftButton.image.color = transparent;
            leftButton.interactable = true;
        }
        if (currentBoxInd == LevelBoxes.Count - 1)
        {
            rightButton.image.color = transparent;
            rightButton.interactable = true;
        }

        // foreach (var box in LevelBoxes)
        // {
        //     RectTransform boxRT = box.LevelContainer;
        //     box.LevelContainer.anchoredPosition = new(boxRT.anchoredPosition.x, boxYRange.x);
        // }

        StartCoroutine(PopUpLevelContainer(currentBoxInd));
    }

    public void NextCam()
    {
        if (currentBoxInd == LevelBoxes.Count - 1) return;
        StartCoroutine(PushDownLevelContainer(currentBoxInd));
        currentBoxInd++;
        DisableAllCamerasExceptOne(currentBoxInd);
        rightButton.image.color = Color.white;
        StartCoroutine(FadeInButtons());
    }

    public void PrevCam()
    {
        if (currentBoxInd == 0) return;
        StartCoroutine(PushDownLevelContainer(currentBoxInd));
        currentBoxInd--;
        DisableAllCamerasExceptOne(currentBoxInd);
        rightButton.image.color = Color.blue;
        StartCoroutine(FadeInButtons());
    }

    private void DisableAllCamerasExceptOne(int index)
    {
        LevelBoxes[index].VirtualCam.enabled = true;
        for (int i = 0; i < LevelBoxes.Count; i++)
        {
            if (i != index) LevelBoxes[i].VirtualCam.enabled = false;
        }
    }

    private void CheckSerializeFieldVariables()
    {
        if (LevelBoxes.Count == 0) Debug.LogError("No Cameras");
        if (!cinemachineBrain) Debug.LogError("Cinemachine Brain is missing");
        if (!leftButton) Debug.LogError("Left Button is missing");
        if (!rightButton) Debug.LogError("Right Button is missing");
    }

    #region Animations
    IEnumerator FadeInButtons()
    {
        Color newColor = Color.white;
        newColor.a = 0f;
        leftButton.image.color = newColor;
        rightButton.image.color = newColor;
        leftButton.interactable = false;
        rightButton.interactable = false;
        float colorDelta = Time.deltaTime / fadeDuration;

        float waitTimeBeforeFade = Mathf.Clamp(cinemachineBrain.m_DefaultBlend.BlendTime - fadeDuration, 0f, cinemachineBrain.m_DefaultBlend.BlendTime);
        yield return new WaitForSeconds(waitTimeBeforeFade);
        while (newColor.a < 1f)
        {
            newColor.a += colorDelta;
            newColor.a = Mathf.Clamp(newColor.a, 0f, 1f);
            if (currentBoxInd > 0) leftButton.image.color = newColor;
            if (currentBoxInd < LevelBoxes.Count - 1) rightButton.image.color = newColor;
            yield return Time.deltaTime;
        }

        if (currentBoxInd > 0) leftButton.interactable = true;
        if (currentBoxInd < LevelBoxes.Count - 1) rightButton.interactable = true;
        StartCoroutine(PopUpLevelContainer(currentBoxInd));
    }

    // IEnumerator SwitchLevelBox()
    // {
    //     float rateToMinYVal = boxYRange -
    //     for (int i = 0; i < LevelBoxes.Count; i++)
    //     {
    //         if (i == currentBoxInd) LevelBoxes[i].LevelContainer.SetActive(true);
    //         else LevelBoxes[i].LevelContainer.SetActive(false);
    //     }

    //     float timer = 0;
    //     while (timer < 0.1f)
    //     {

    //     }
    // }

    IEnumerator PopUpLevelContainer(int boxInd)
    {
        LevelBoxes[boxInd].LevelContainer.gameObject.SetActive(true);
        yield return null;

        // Below method causes lag spike
        // RectTransform box = LevelBoxes[boxInd].LevelContainer;
        // box.anchoredPosition = new(box.anchoredPosition.x, boxYRange.x);
        // Vector2 vecDelta = new(0, (boxYRange.y - boxYRange.x) / pushPopDuration);
        // float timer = 0;
        // while (timer < pushPopDuration)
        // {
        //     box.anchoredPosition += vecDelta * Time.deltaTime;
        //     timer += Time.deltaTime;
        //     yield return Time.deltaTime;
        // }
    }
    
    IEnumerator PushDownLevelContainer(int boxInd)
    {
        LevelBoxes[boxInd].LevelContainer.gameObject.SetActive(false);
        yield return null;

        // Below method causes lag spike
        // RectTransform box = LevelBoxes[boxInd].LevelContainer;
        // box.anchoredPosition = new(box.anchoredPosition.x, boxYRange.y);
        // Vector2 vecDelta = new(0, (boxYRange.x - boxYRange.y) / pushPopDuration);
        // float timer = 0;
        // while (timer < pushPopDuration)
        // {
        //     box.anchoredPosition -= vecDelta * Time.deltaTime;
        //     timer += Time.deltaTime;
        //     yield return Time.deltaTime;
        // }
        // box.anchoredPosition = new(box.anchoredPosition.x, boxYRange.x);
    }
    #endregion
}