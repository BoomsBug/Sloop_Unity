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
        
        [Header("Labels")]
        [SerializeField] private TMP_Text masterValue;
        [SerializeField] private TMP_Text musicValue;
        [SerializeField] private TMP_Text sfxValue;

        private void Start()
        {
            masterSlider.value = AudioManager.Instance.MasterVolume;
            musicSlider.value = AudioManager.Instance.MusicVolume;
            sfxSlider.value = AudioManager.Instance.SFXVolume;
            UpdateLabels();

            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        void SetMasterVolume(float value)
        {
            AudioManager.Instance.SetMasterVolume(value);
            UpdateLabels();
        }

        void SetMusicVolume(float value)
        {
            AudioManager.Instance.SetMusicVolume(value);
            UpdateLabels();
        }

        void SetSFXVolume(float value)
        {
            AudioManager.Instance.SetSFXVolume(value);
            UpdateLabels();
        }

        void UpdateLabels()
        {
            if (masterValue) masterValue.text=Mathf.RoundToInt(masterSlider.value*100)+ "%";
            if (musicValue) musicValue.text=Mathf.RoundToInt(musicSlider.value*100)+ "%";
            if (sfxValue) sfxValue.text=Mathf.RoundToInt(sfxSlider.value*100)+ "%";
        }

        public void Close()
        {
            UIManager.Instance.CloseTopPanel();
        }
    }
}