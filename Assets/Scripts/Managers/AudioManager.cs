using AlpacaMyGames;
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
                clipToPlay = _audioContainer.BulletHitsCharacter.GetRandomElement();
                break;
            case SFXClip.Crafting:
                clipToPlay = _audioContainer.CraftingSound.GetRandomElement();
                break;
            case SFXClip.KeyboardTyping:
                clipToPlay = _audioContainer.KeyboardTypeing3s;
                break;
            case SFXClip.BoxSmash:
                clipToPlay = _audioContainer.BoxSmash;
                break;
            case SFXClip.PortalSound:
                clipToPlay = _audioContainer.PortalSound;
                break;
            case SFXClip.BushRattle:
                clipToPlay = _audioContainer.BushRattle.GetRandomElement();
                break;
            case SFXClip.PatchingSound:
                clipToPlay = _audioContainer.PatchingSounds.GetRandomElement();
                break;
            case SFXClip.GunLoad:
                clipToPlay = _audioContainer.GunLoading.GetRandomElement();
                break;
            case SFXClip.Lockpicking:
                clipToPlay = _audioContainer.Lockpicking.GetRandomElement();
                break;
            default:
                clipToPlay = null;
                break;
        }

        if (clipToPlay != null)
            _sfxAudioSource.PlayOneShot(clipToPlay);
    }

    public void StopPlaying()
    {
        _sfxAudioSource.Stop();
    }
}