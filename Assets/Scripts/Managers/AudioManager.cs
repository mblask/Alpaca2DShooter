using AlpacaMyGames;
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
        if (_instance != null)
            Destroy(_instance.gameObject);

        _instance = this;
        DontDestroyOnLoad(this);

        _audioContainer = transform.Find("AudioContainer").GetComponent<AudioContainer>();

        _musicAudioSource = transform.Find("MusicOutput").GetComponent<AudioSource>();
        _sfxAudioSource = transform.Find("SFXOutput").GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (!_playMusic)
            _musicAudioSource.Stop();
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

    public void PlayClip(SFXClip audioClip)
    {
        AudioClip clipToPlay = getSfxClip(audioClip);

        if (clipToPlay != null)
            _sfxAudioSource.PlayOneShot(clipToPlay);
    }

    public void StopPlaying()
    {
        _sfxAudioSource.Stop();
    }

    private AudioClip getSfxClip(SFXClip audioClip)
    {
        switch (audioClip)
        {
            case SFXClip.GunShot:
                return _audioContainer.Gunshot1;
            case SFXClip.SilencerShot:
                return _audioContainer.SilencedShot;
            case SFXClip.MachinegunShot:
                return _audioContainer.Gunshot2;
            case SFXClip.ShotgunShot:
                return _audioContainer.ShotgunShot;
            case SFXClip.GunReload:
                return _audioContainer.GunReload;
            case SFXClip.MachinegunReload:
                return _audioContainer.MachineGunReload;
            case SFXClip.ShotgunReload:
                return _audioContainer.ShotgunReload;
            case SFXClip.ItemPickup:
                return _audioContainer.ItemPickup;
            case SFXClip.Bandaging:
                return _audioContainer.Bandaging;
            case SFXClip.BulletHitsCharacter:
                return _audioContainer.BulletHitsCharacter.GetRandomElement();
            case SFXClip.Crafting:
                return _audioContainer.CraftingSound.GetRandomElement();
            case SFXClip.KeyboardTyping:
                return _audioContainer.KeyboardTypeing3s;
            case SFXClip.BoxSmash:
                return _audioContainer.BoxSmash;
            case SFXClip.PortalSound:
                return _audioContainer.PortalSound;
            case SFXClip.BushRattle:
                return _audioContainer.BushRattle.GetRandomElement();
            case SFXClip.PatchingSound:
                return _audioContainer.PatchingSounds.GetRandomElement();
            case SFXClip.GunLoad:
                return _audioContainer.GunLoading.GetRandomElement();
            case SFXClip.Lockpicking:
                return _audioContainer.Lockpicking.GetRandomElement();
            default:
                return null;
        }
    }
}