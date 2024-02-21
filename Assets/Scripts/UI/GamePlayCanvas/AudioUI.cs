using UnityEngine;
using UnityEngine.UI;

public class AudioUI : MonoBehaviour
{
    private Slider _masterVolumeSlider;
    private Slider _musicVolumeSlider;
    private Slider _sfxVolumeSlider;

    private AudioManager _audioManager;

    private void Awake()
    {
        _masterVolumeSlider = transform.Find("Container").Find("MasterSlider").GetComponent<Slider>();
        _musicVolumeSlider = transform.Find("Container").Find("MusicSlider").GetComponent<Slider>();
        _sfxVolumeSlider = transform.Find("Container").Find("SFXSlider").GetComponent<Slider>();
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;
        setupInitialVolume();
    }

    private void setupInitialVolume(float masterValue = 50, float musicValue = 50, float sfxValue = 50)
    {
        _audioManager.MasterVolume = calculateDecibels(masterValue);
        _masterVolumeSlider.value = masterValue;
        _audioManager.MusicVolume = calculateDecibels(musicValue);
        _musicVolumeSlider.value = musicValue;
        _audioManager.SFXVolume = calculateDecibels(sfxValue);
        _sfxVolumeSlider.value = sfxValue;
    }

    public void SetupMasterVolume()
    {
        _audioManager.MasterVolume = calculateDecibels(_masterVolumeSlider.value);
    }

    public void SetupMusicVolume()
    {
        _audioManager.MusicVolume = calculateDecibels(_musicVolumeSlider.value);
    }

    public void SetupSFXVolume()
    {
        _audioManager.SFXVolume = calculateDecibels(_sfxVolumeSlider.value);
    }

    //min -80, max 5 decibels
    private float calculateDecibels(float sliderValue)
    {
        if (sliderValue < 1.0f)
            return -80.0f;

        return -75 + 40 * Mathf.Log10(sliderValue);
    }
}
