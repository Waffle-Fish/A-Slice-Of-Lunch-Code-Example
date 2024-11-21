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
        AudioManager.Instance.MusicVolume((float) Math.Round(_musicSlider.value, 1));
    }
    
    public void SFXVolume() {
        AudioManager.Instance.SFXVolume((float) Math.Round(_sfxSlider.value, 1));
    }
}
