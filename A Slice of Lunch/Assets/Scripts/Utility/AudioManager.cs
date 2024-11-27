using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}
    [Min(0)]
    public int trackToPlay;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

   

    public void Awake()
    { 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start() {
        // PlayMusic(trackToPlay);

        StartCoroutine(PlayMenuTheme());
    }

    public IEnumerator PlayMenuTheme() {
        Debug.Log("Play the Main Menu Theme sequence!");
        PlayMusic("MenuThemeIntro");
        yield return new WaitForSeconds(musicSource.clip.length);
        Debug.Log("Play the main loop.");
        PlayMusic("MenuThemeLoop");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlayMusic(int ind)
    {
        Sound s = musicSounds[ind];

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlayMusic(AudioClip music) {
        if (music == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = music;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void PlaySFX(AudioClip sfx)
    {
        if (sfx == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            sfxSource.PlayOneShot(sfx);
        }
    }

    public void ToggelMusic() {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX() {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume) {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume) {
        sfxSource.volume = volume;
    }

    public void UpdateTrack(int trackNumber) {
        if (trackNumber == 0) {
            StartCoroutine(PlayMenuTheme());
            Debug.Log("Now Playing the Menu Theme Sequence");
        } else if (trackNumber == 2) {
            PlayMusic("LevelTheme");
            Debug.Log("Now Playing the Level Theme");
        }
    }
}
