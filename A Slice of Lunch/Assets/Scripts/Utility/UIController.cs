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

    public void Start()
    {
        if (AudioManager.Instance != null)
        {
            // _musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            // _sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
            _musicSlider.value = 0.4f;
            _sfxSlider.value = 0.5f;
            MusicVolume();
            SFXVolume();
        }

        if (isInGameSettings == true)
        {
            _settingsPanel.SetActive(false);
            Debug.Log("Deactivated settings panel");
        }

        Debug.Log("Loaded volume levels");
    }

    public void Update()
    {
        _musicVolumeText.text = PercentToString(Math.Round(_musicSlider.value, 1));
        _sfxVolumeText.text = PercentToString(Math.Round(_sfxSlider.value, 1));
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        float musicVolume = _musicSlider.value;
        AudioManager.Instance.MusicVolume(musicVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);

        Debug.Log("Saved music volume level");
    }

    public void SFXVolume()
    {
        float sfxVolume = _sfxSlider.value;
        AudioManager.Instance.SFXVolume(sfxVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        // AudioManager.Instance.PlaySFX("Slice");
        Debug.Log("Saved SFX volume level");
    }

    public void PauseGame()
    {
        AudioManager.Instance.PlaySFX("ButtonPop");
        if (!isGamePaused)
        {
            isGamePaused = true;
            _settingsPanel.SetActive(true);
            // Time.timeScale = 0f;

            Debug.Log("Pause Game");
        }
        else if (isGamePaused)
        {
            isGamePaused = false;
            _settingsPanel.SetActive(false);
            // Time.timeScale = 1f;

            Debug.Log("Resume Game");
        }
    }

    private String PercentToString(double percent)
    {
        return (percent * 100).ToString("F0") + "%";
    }
}
