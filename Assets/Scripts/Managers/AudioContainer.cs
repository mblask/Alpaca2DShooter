using System.Collections.Generic;
using UnityEngine;

public class AudioContainer : MonoBehaviour
{
    [Header("Weapon shooting")]
    public AudioClip Gunshot1;
    public AudioClip Gunshot2;
    public AudioClip SilencedShot;
    public AudioClip ShotgunShot;

    [Header("Weapon reloading")]
    public AudioClip GunReload;
    public List<AudioClip> GunLoading = new List<AudioClip>();
    public AudioClip MachineGunReload;
    public List<AudioClip> MachineGunLoading = new List<AudioClip>();
    public AudioClip ShotgunReload;

    [Header("Other")]
    public AudioClip ItemPickup;
    public AudioClip Bandaging;
    public AudioClip BoxSmash;
    public AudioClip KeyboardTypeing3s;
    public AudioClip PortalSound;
    public List<AudioClip> PatchingSounds = new List<AudioClip>();
    public List<AudioClip> BushRattle = new List<AudioClip>();
    public List<AudioClip> CraftingSound = new List<AudioClip>();
    public List<AudioClip> WoodenDoorHits = new List<AudioClip>();
    public List<AudioClip> BulletHitsCharacter = new List<AudioClip>();
}
