using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private const float VOLUME_DIVISION_CONST = 10.0f;
    private const float VOLUME_ABS_MINIMUM = 8.01f;
    private const float VOLUME_WIDTH = 1.0f;

    [SerializeField] private AudioMixer _mainAudioMixer;

    private AudioContainer _audioContainer;

    private AudioSource _musicAudioSource;
    private AudioSource _sfxAudioSource;

    private const string MASTER_VOLUME_STRING = "MasterVolume";
    private const string MUSIC_VOLUME_STRING = "MusicVolume";
    private const string SFX_VOLUME_STRING = "SFXVolume";

    [SerializeField] private bool _playMusic = true;

    public float MasterVolume
    {
        get
        {
            _mainAudioMixer.GetFloat(MASTER_VOLUME_STRING, out float value);
            return value;
        }

        set
        {
            _mainAudioMixer.SetFloat(MASTER_VOLUME_STRING, value);
        }
    }

    public float MusicVolume
    {
        get
        {
            _mainAudioMixer.GetFloat(MUSIC_VOLUME_STRING, out float value);
            return value;
        }

        set
        {
            _mainAudioMixer.SetFloat(MUSIC_VOLUME_STRING, value);
        }
    }

    public float SFXVolume
    {
        get
        {
            _mainAudioMixer.GetFloat(SFX_VOLUME_STRING, out float value);
            return value;
        }

        set
        {
            _mainAudioMixer.SetFloat(SFX_VOLUME_STRING, value);
        }
    }

    public void Awake()
    {
        _instance = this;

        _audioContainer = transform.Find("AudioContainer").GetComponent<AudioContainer>();

        _musicAudioSource = transform.Find("MusicOutput").GetComponent<AudioSource>();
        _sfxAudioSource = transform.Find("SFXOutput").GetComponent<AudioSource>();
    }

    private void Start()
    {
        Bullet.OnBulletHitsCharacter += playClip;
        if (PlayerWeapons.Instance != null)
        {
            PlayerWeapons.Instance.OnShootingAudio += playClip;
            PlayerWeapons.Instance.OnReloadingAudio += playClip;
        }
        NPCWeapons.OnEnemyShootingAudio += playClip;
        PickupItem.OnItemPickedUpAudio += playClip;
        FiringTrap.OnWeaponShootingAudio += playClip;
        WallFiringTrap.OnShooting += playClip;

        if (!_playMusic)
            _musicAudioSource.Stop();
    }

    private void OnDisable()
    {
        Bullet.OnBulletHitsCharacter -= playClip;
        if (PlayerWeapons.Instance != null)
        {
            PlayerWeapons.Instance.OnShootingAudio -= playClip;
            PlayerWeapons.Instance.OnReloadingAudio -= playClip;
        }
        NPCWeapons.OnEnemyShootingAudio -= playClip;
        PickupItem.OnItemPickedUpAudio -= playClip;
        FiringTrap.OnWeaponShootingAudio -= playClip;
        WallFiringTrap.OnShooting -= playClip;
    }

    public float GetMusicVolume()
    {
        _mainAudioMixer.GetFloat(MUSIC_VOLUME_STRING, out float musicVolume);
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        _mainAudioMixer.GetFloat(SFX_VOLUME_STRING, out float sfxVolume);

        return sfxVolume;
    }

    private void playClip(SFXClip audioClip)
    {
        AudioClip clipToPlay;

        switch (audioClip)
        {
            case SFXClip.GunShot:
                clipToPlay = _audioContainer.Gunshot1;
                break;
            case SFXClip.SilencerShot:
                clipToPlay = _audioContainer.SilencedShot;
                break;
            case SFXClip.MachinegunShot:
                clipToPlay = _audioContainer.Gunshot2;
                break;
            case SFXClip.ShotgunShot:
                clipToPlay = _audioContainer.ShotgunShot;
                break;
            case SFXClip.GunReload:
                clipToPlay = _audioContainer.GunReload;
                break;
            case SFXClip.MachinegunReload:
                clipToPlay = _audioContainer.MachineGunReload;
                break;
            case SFXClip.ShotgunReload:
                clipToPlay = _audioContainer.ShotgunReload;
                break;
            case SFXClip.ItemPickup:
                clipToPlay = _audioContainer.ItemPickup;
                break;
            case SFXClip.Bandaging:
                clipToPlay = _audioContainer.Bandaging;
                break;
            case SFXClip.BulletHitsCharacter:
                clipToPlay = _audioContainer.BulletHitsCharacter[UnityEngine.Random.Range(0, _audioContainer.BulletHitsCharacter.Count)];
                break;
            default:
                clipToPlay = null;
                break;
        }

        if (clipToPlay != null)
            _sfxAudioSource.PlayOneShot(clipToPlay);
    }
}