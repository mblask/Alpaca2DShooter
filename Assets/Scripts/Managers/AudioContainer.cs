using System.Collections;
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
    public AudioClip MachineGunReload;
    public AudioClip ShotgunReload;

    [Header("Other")]
    public AudioClip ItemPickup;
    public AudioClip Bandaging;
    public List<AudioClip> BulletHitsCharacter;
}
