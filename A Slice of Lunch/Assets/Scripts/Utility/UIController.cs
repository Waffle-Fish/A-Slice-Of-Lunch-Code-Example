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
    public Button _pauseButton;
    public GameObject _settingsPanel;
    public bool isInGameSettings;
    bool isGamePaused = false;

    public void Start() {
        if (AudioManager.Instance != null) {
            _musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            _sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        }
        
        if (isInGameSettings == true) {
            _settingsPanel.SetActive(false);
            Debug.Log("Deactivated settings panel");
        }

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

    public void PauseGame() {
        AudioManager.Instance.PlaySFX("ButtonPop");
        if (!isGamePaused) {
            isGamePaused = true;
            _settingsPanel.SetActive(true);
            // Time.timeScale = 0f;

            Debug.Log("Pause Game");
        } else if (isGamePaused) {
            isGamePaused = false;
            _settingsPanel.SetActive(false);
            // Time.timeScale = 1f;

            Debug.Log("Resume Game");
        }
    }

}
