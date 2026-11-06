using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioCollection : MonoBehaviour
{
    [Header("========== Output ==========")]
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource SFX;

    [Header("========== Background Music ==========")]
    public AudioClip mainMenu;
    public AudioClip gameScene;

    [Header("========== SFX ==========")]
    public AudioClip buttonClick;
    public AudioClip chatNotification;
    public AudioClip messageSound;

    private static AudioCollection instance;
    private List<AudioSource> sfxSources = new List<AudioSource>();

    public static AudioCollection GetInstance()
    {
        if (instance == null)
        {
            instance = FindFirstObjectByType<AudioCollection>();
        }
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        Debug.Log("Playing BGM: " + clip.name);
        BGM.clip = clip;
        BGM.loop = true;
        BGM.Play();
    }

    public void StopPlayBGM()
    {
        BGM.Stop();
    }

    public void PauseBGM()
    {
        if (BGM.isPlaying)
        {
            BGM.Pause();
        }
    }

    public void ResumeBGM()
    {
        if (!BGM.isPlaying)
        {
            BGM.UnPause();
        }
    }

    public AudioSource PlaySFX(AudioClip clip, float volume = 1f)
    {
        AudioSource availableSource = GetAvailableAudioSource();
        availableSource.clip = clip;
        availableSource.volume = volume;
        availableSource.Play();
        return availableSource;
    }

    public void StopSFX()
    {
        foreach (var source in sfxSources)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        sfxSources.Add(newSource);
        return newSource;
    }
}