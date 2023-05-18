using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioUI : MonoBehaviour
{
    //private Slider _musicVolumeSlider;
    private Slider _sfxVolumeSlider;

    private AudioManager _audioManager;

    private void Awake()
    {
        //_musicVolumeSlider = transform.Find("Container").Find("MusicSlider").GetComponent<Slider>();
        _sfxVolumeSlider = transform.Find("Container").Find("SFXSlider").GetComponent<Slider>();
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;
        SetupAudioSliders(_audioManager);
    }

    private void SetupAudioSliders(AudioManager audioManager)
    {
        //setupMusicSlider(audioManager.GetMusicVolume());
        setupSFXSlider(audioManager.GetSFXVolume());
    }

    private void setupMusicSlider(float value)
    {
        //_musicVolumeSlider.value = value;
    }

    private void setupSFXSlider(float value)
    {
        _sfxVolumeSlider.value = value;
    }
}
