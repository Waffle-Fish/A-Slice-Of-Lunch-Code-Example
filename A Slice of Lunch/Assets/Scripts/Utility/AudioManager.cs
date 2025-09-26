using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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

        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
        if (!musicSource) musicSource = audioSources[0];
        if (!sfxSource) sfxSource = audioSources[1];

        MusicVolume(0.3f);
        SFXVolume(0.5f);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += UpdateTrack;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= UpdateTrack;
    }

    public void Start() {
        if (trackToPlay > 0 || trackToPlay < musicSounds.Length)
        {
            // Debug.Log("Play given track");
            PlayMusic(trackToPlay);
        }
        else
        {
            // Debug.Log("Play menu track");
            StartCoroutine(PlayMenuTheme());
        }
    }

    public IEnumerator PlayMenuTheme() {
        // Debug.Log("Play the Main Menu Theme sequence!");
        PlayMusic("MenuThemeIntro");
        yield return new WaitForSeconds(musicSource.clip.length);
        // Debug.Log("Play the main loop.");
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
            sfxSource.Stop();
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

    public void ToggelMusic() 
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX() 
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume) 
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume) 
    {
        sfxSource.volume = volume;
    }

    // Play level sound track based on the level's build index
    public void UpdateTrack(Scene scene, LoadSceneMode mode) 
    {
        string currentScene = scene.name;
        int currentLevel = (int)currentScene[currentScene.Length - 1];

        Debug.Log("For playing the level music: Current Scene '" + currentScene + "'.");
        if (!musicSource.clip) Debug.Log("MusicSource.clip is gone");
        else Debug.Log("MusicSource.clip is here with name: " + musicSource.clip);


        if (musicSource.clip.name == "IntroStory" && !musicSource.loop)
        {
            musicSource.loop = true;
        }

        // Set the case to the first five letters of the cuisine
        // Ex: America --> case "Ameri":
        switch (currentScene.Substring(0, 5))
        {
            case "Italy":
                if (currentLevel > 0 && currentLevel < 5 && !CheckIfTrackIsAlreadyPlaying("UpbeatNoodles"))
                    PlayMusic("DownbeatNoodles");    // PlayMusic("UpbeatNoodles");
                else if (!CheckIfTrackIsAlreadyPlaying("DownbeatNoodles"))
                    PlayMusic("UpbeatNoodles");      // PlayMusic("DownbeatNoodles");
                Debug.Log("Playing Italy Music!");
                break;
            case "Japan":
                if (!CheckIfTrackIsAlreadyPlaying("UpbeatJapan"))
                    PlayMusic("UpbeatJapan");
                Debug.Log("Playing Japan Music!");
                break;
            case "Intro":
                if (!CheckIfTrackIsAlreadyPlaying("IntroStory"))
                {
                    PlayMusic("IntroStory");
                    musicSource.loop = false;
                }
                break;
            default:
                if (!CheckIfTrackIsAlreadyPlaying("MenuTheme"))
                {
                    Debug.Log("MenuTheme is not playing, so start playing it!");
                    PlayMusic("MenuTheme");
                }
                break;
        }
    }

    private bool CheckIfTrackIsAlreadyPlaying(string track)
    {
        Debug.Log("Is " + track + " already playing? CurrentTrack: " + musicSource.clip.name);
        return musicSource.clip.name == track;
    }
}
