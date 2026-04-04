using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

namespace Sloop.UI
{
    public class SettingsPanel : UIPanel
    {
        [Header("Audio Sliders")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            masterSlider.value = MusicManager.Instance.MasterVolume;
            musicSlider.value = MusicManager.Instance.SongVolume;
            sfxSlider.value = MusicManager.Instance.SFXVolume;

            masterSlider.onValueChanged.AddListener(MusicManager.Instance.SetMasterVolume);
            musicSlider.onValueChanged.AddListener(MusicManager.Instance.SetSongVolume);
            sfxSlider.onValueChanged.AddListener(MusicManager.Instance.SetSFXVolume);
        }
    }
}