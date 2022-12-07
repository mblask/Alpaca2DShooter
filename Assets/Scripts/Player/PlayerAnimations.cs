using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    Idle,
    Gun,
    Silencer,
    Machine,
    Shotgun,
    Reload,
    RemoveWeapon,
}

public class PlayerAnimations : Singleton<PlayerAnimations>
{
    private const string EQUIP_GUN = "EquipGun";
    private const string EQUIP_SILENCER = "EquipSilencer";
    private const string EQUIP_MACHINE = "EquipMachine";
    private const string EQUIP_SHOTGUN = "EquipShotgun";
    private const string RELOAD = "Reload";
    private const string REMOVE_WEAPON = "RemoveWeapon";
    
    private Animator _animator;

    private AnimationType _currentAnimationType;

    public override void Awake()
    {
        base.Awake();

        _animator = GetComponent<Animator>();
        _currentAnimationType = AnimationType.Idle;
    }

    private void Start()
    {
        if (_animator.runtimeAnimatorController == null)
            _animator.runtimeAnimatorController = GameAssets.Instance.CharacterBaseList[0].CharacterAOC;
    }

    public void SetPlayerAOC(AnimatorOverrideController aoc)
    {
        _animator.runtimeAnimatorController = aoc;
    }

    public void PlayAnimation(AnimationType animation)
    {
        if (IsCurrentAnimationWeapon() && IsAnimationWeapon(animation))
            _animator.SetTrigger(REMOVE_WEAPON);

        _currentAnimationType = animation;

        switch (animation)
        {
            case AnimationType.Idle:
                break;
            case AnimationType.Gun:
                _animator.SetTrigger(EQUIP_GUN);
                break;
            case AnimationType.Silencer:
                _animator.SetTrigger(EQUIP_SILENCER);
                break;
            case AnimationType.Machine:
                _animator.SetTrigger(EQUIP_MACHINE);
                break;
            case AnimationType.Shotgun:
                _animator.SetTrigger(EQUIP_SHOTGUN);
                break;
            case AnimationType.Reload:
                _animator.SetTrigger(RELOAD);
                break;
            case AnimationType.RemoveWeapon:
                _animator.SetTrigger(REMOVE_WEAPON);
                break;
        }
    }

    public bool IsAnimationWeapon(AnimationType currentAnimationType)
    {
        switch (currentAnimationType)
        {
            case AnimationType.Gun:
            case AnimationType.Silencer:
            case AnimationType.Machine:
            case AnimationType.Shotgun:
                return true;
            default:
                return false;
        }
    }

    public bool IsCurrentAnimationWeapon()
    {
        return IsAnimationWeapon(_currentAnimationType);
    }

    public void SetBool(string name, bool value)
    {
        _animator.SetBool(name, value);
    }

    public bool GetBool(string name)
    {
        return _animator.GetBool(name);
    }
}
