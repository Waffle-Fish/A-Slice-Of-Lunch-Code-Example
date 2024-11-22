using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

    // public IPanel SettingsPanel;

    private bool showSettingsMenu = false;

    public void ToggleSettingsMenu() {
        // SettingsPanel.SetActive(showSettingsMenu);
    }

    public void ToggleMusic() {
        AudioManager.Instance.ToggelMusic();
    }

    public void ToggleSFX() {
        AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume() {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }
    
    public void SFXVolume() {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
    }
}
