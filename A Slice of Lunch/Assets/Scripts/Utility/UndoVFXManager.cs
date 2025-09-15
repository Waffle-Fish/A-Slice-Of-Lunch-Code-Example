using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[RequireComponent(typeof(PlayableDirector), typeof(Image))]
public class UndoVFXManager : MonoBehaviour
{
    PlayableDirector pd;
    Image image;

    private void Awake()
    {
        pd = GetComponent<PlayableDirector>();
        image = GetComponent<Image>();

        image.raycastTarget = false;
    }

    private void OnEnable()
    {
        UndoButton.OnUndoButtonPressed += PlayVFX;
    }

    private void OnDisable()
    {
        UndoButton.OnUndoButtonPressed -= PlayVFX;
    }

    private void PlayVFX()
    {
        IEnumerator ProcessVFX()
        {
            PlayerInputManager.Instance.ToggleControls(false);
            image.raycastTarget = true;
            pd.Play();
            // play audio
            yield return new WaitForSeconds((float)pd.playableAsset.duration);
            PlayerInputManager.Instance.ToggleControls(true);
            image.raycastTarget = false;
        }

        StartCoroutine(ProcessVFX());
    }
}
