using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioMixer mixer;
    public float MasterVolume {get; private set;} = 1f;
    public float MusicVolume {get; private set;} = 1f;
    public float SFXVolume {get; private set;} = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance=this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        mixer.SetFloat("MasterVolume", Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("MasterVolume",value);
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        mixer.SetFloat("MusicVolume", Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("MusicVolume",value);
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = value;
        mixer.SetFloat("SFXVolume", Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("SFXVolume",value);
    }

    private void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume",1f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume",1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume",1f);

        SetMasterVolume(MasterVolume);
        SetMusicVolume(MusicVolume);
        SetSFXVolume(SFXVolume);
    }
}
