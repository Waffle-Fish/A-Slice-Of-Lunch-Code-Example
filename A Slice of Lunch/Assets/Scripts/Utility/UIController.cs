using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;
    public TMP_Text _musicVolumeText, _sfxVolumeText;

    public void Start() {
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        _sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        Debug.Log("Loaded volume levels");
    }

    public void Update() {
        _musicVolumeText.text = Math.Round(_musicSlider.value, 1).ToString();
        _sfxVolumeText.text = Math.Round(_sfxSlider.value, 1).ToString();
    }

    public void ToggleMusic() {
        AudioManager.Instance.ToggelMusic();
    }

    public void ToggleSFX() {
        AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume() {
        float musicVolume = (float) Math.Round(_musicSlider.value, 1);
        AudioManager.Instance.MusicVolume(musicVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);

        Debug.Log("Saved music volume level");
    }
    
    public void SFXVolume() {
        float sfxVolume = (float) Math.Round(_sfxSlider.value, 1);
        AudioManager.Instance.SFXVolume(sfxVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);

        Debug.Log("Saved SFX volume level");
    }
}
