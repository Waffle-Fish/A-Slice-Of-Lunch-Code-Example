using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestSFX : MonoBehaviour
{
    [SerializeField] string sfx;
    [SerializeField] AudioClip SFXClip;
    [SerializeField] AudioSource MenuThemeIntro;
    [SerializeField] AudioSource MenuThemeLoop;

    void Start() {
        MenuThemeIntro.Play();
        MenuThemeLoop.PlayDelayed(MenuThemeIntro.clip.length);
    }

    void OnMouseDown(){
        if (AudioManager.Instance == null) {
            Debug.Log("Missing Audio Manager");
        }
        else {
            Debug.Log("Sprite Clicked");
            AudioManager.Instance.PlaySFX(SFXClip);
        }
    }
}
