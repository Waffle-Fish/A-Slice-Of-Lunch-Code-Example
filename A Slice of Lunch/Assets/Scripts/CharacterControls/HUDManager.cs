using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    TextMeshProUGUI[] textMeshProUGUIs;
    TextMeshProUGUI sliceTMP;

    

    private void Awake() {
        textMeshProUGUIs = GetComponentsInChildren<TextMeshProUGUI>();
        sliceTMP = textMeshProUGUIs[0];
    }

    private void OnEnable() {
        PlayerSlice.OnSliceCountChange += UpdateSliceCountText;
    }

    private void OnDisable() {
        PlayerSlice.OnSliceCountChange -= UpdateSliceCountText;
    }

    private void UpdateSliceCountText(int currentSliceCount)
    {
        sliceTMP.text = $"Slices Left: {currentSliceCount}";
    }
}
