using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Min(0)]
    public int trackToPlay;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private Coroutine introRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto-assign AudioSources if not set
        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
        if (!musicSource && audioSources.Length > 0) musicSource = audioSources[0];
        if (!sfxSource && audioSources.Length > 1) sfxSource = audioSources[1];

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

    private void Start()
    {
        // if (trackToPlay >= 0 && trackToPlay < musicSounds.Length)
        // {
        //     PlayMusic(trackToPlay, true);
        // }
        // else
        // {
        //     StartCoroutine(PlayMenuTheme());
        // }
        StartCoroutine(PlayMenuTheme());
    }

    private IEnumerator PlayMenuTheme()
    {
        PlayMusic("MenuThemeIntro", false);
        if (musicSource.clip != null)
        {
            yield return new WaitForSeconds(musicSource.clip.length);
        }
        PlayMusic("MenuThemeLoop", true);
    }

    private IEnumerator PlayIntroThenMenuLoop()
    {
        PlayMusic("IntroStory", false);

        if (musicSource.clip != null)
        {
            yield return new WaitForSeconds(musicSource.clip.length);
        }

        PlayMusic("MenuThemeLoop", true);
    }

    public void PlayMusic(string name, bool loop = true)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning($"[AudioManager] Music '{name}' not found.");
            return;
        }

        musicSource.clip = s.clip;
        musicSource.loop = loop;
        musicSource.Play();

        // Debug.Log($"[AudioManager] Now playing: {name} (Loop={loop})");
    }

    public void PlayMusic(int index, bool loop = true)
    {
        if (index < 0 || index >= musicSounds.Length)
        {
            Debug.LogWarning($"[AudioManager] Invalid music index {index}");
            return;
        }

        Sound s = musicSounds[index];
        musicSource.clip = s.clip;
        musicSource.loop = loop;
        musicSource.Play();

        // Debug.Log($"[AudioManager] Now playing index {index}: {s.name} (Loop={loop})");
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning($"[AudioManager] SFX '{name}' not found.");
            return;
        }

        sfxSource.PlayOneShot(s.clip);
        // Debug.Log($"[AudioManager] Played SFX: {name}");
    }

    public void PlaySFX(AudioClip sfx)
    {
        if (sfx == null)
        {
            Debug.LogWarning("[AudioManager] Null SFX clip passed to PlaySFX.");
            return;
        }

        sfxSource.PlayOneShot(sfx);
        // Debug.Log($"[AudioManager] Played SFX clip: {sfx.name}");
    }

    public void ToggleMusic() => musicSource.mute = !musicSource.mute;
    public void ToggleSFX() => sfxSource.mute = !sfxSource.mute;

    public void MusicVolume(float volume) => musicSource.volume = Mathf.Clamp01(volume);
    public void SFXVolume(float volume) => sfxSource.volume = Mathf.Clamp01(volume);

    /// <summary>
    /// Called automatically whenever a new scene loads.
    /// </summary>
    private void UpdateTrack(Scene scene, LoadSceneMode mode)
    {
        string currentScene = scene.name;
        // Debug.Log($"[AudioManager] Scene loaded: {currentScene}");

        // Stop any intro coroutine if switching scenes
        if (introRoutine != null)
        {
            StopCoroutine(introRoutine);
            introRoutine = null;
        }

        if (currentScene.StartsWith("Intro"))
        {
            if (!CheckIfTrackIsAlreadyPlaying("IntroStory"))
                PlayMusic("IntroStory", false);
        }
        else if (currentScene.StartsWith("Italy"))
        {
            if (!CheckIfTrackIsAlreadyPlaying("UpbeatNoodles"))
                PlayMusic("UpbeatNoodles", true);
        }
        else if (currentScene.StartsWith("Japan"))
        {
            if (!CheckIfTrackIsAlreadyPlaying("UpbeatJapan"))
                PlayMusic("UpbeatJapan", true);
        }
        else
        {
            if (musicSource.clip == null)
            {
                introRoutine = StartCoroutine(PlayMenuTheme());
            }
            else if (!CheckIfTrackIsAlreadyPlaying("MenuThemeLoop"))
            {
                // If we're on MenuThemeIntro (not loop yet), do nothing — it'll transition automatically
                // If we're on a different track, then switch
                if (musicSource.clip.name != "MenuThemeIntro")
                {
                    introRoutine = StartCoroutine(PlayMenuTheme());
                }
            }
        }
    }

    private bool CheckIfTrackIsAlreadyPlaying(string track)
    {
        if (musicSource.clip == null) return false;
        return musicSource.clip.name == track;
    }
}
