using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RedBulletsSkill : ActiveSkill, ISkill
{
    [SerializeField]
    protected FireWeapon _mainWeapon;
    [SerializeField]
    protected FireWeapon _doubleHandedWeapon;
    [SerializeField]
    protected GameObject _projectile;
    [SerializeField]
    protected GameObject _redProjectile;
    [SerializeField]
    private float _damageModificator = 2f;
    private float _normalCooldown;
    [SerializeField]
    private float _attackCooldownBuff = 0.15f;
    [SerializeField]
    private float _buffDuration = 30f;
    [SyncVar(hook = nameof(OnUseSkill))]
    private bool _isUseSkill = false;
    private Character _character;

    private new void Start()
    {
        base.Start();
        _character = GetComponent<Character>();
        _normalCooldown = _mainWeapon.CooldownSeconds;
    }

    private void OnUseSkill(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            Buff();
            return;
        }
        Debuff();
        
    }

    private void Buff()
    {
        _mainWeapon.Projectile = _redProjectile;
        _mainWeapon.Damage *= _damageModificator;
        _mainWeapon.CooldownComponent.CooldownSeconds = _attackCooldownBuff;
        _doubleHandedWeapon.Projectile = _redProjectile;
        _doubleHandedWeapon.Damage *= _damageModificator;
        _doubleHandedWeapon.CooldownComponent.CooldownSeconds = _attackCooldownBuff;
    }

    private void Debuff()
    {
        _mainWeapon.Projectile = _projectile;
        _mainWeapon.Damage /= _damageModificator;
        _mainWeapon.CooldownComponent.CooldownSeconds = _normalCooldown;
        _doubleHandedWeapon.Projectile = _projectile;
        _doubleHandedWeapon.Damage /= _damageModificator;
        _doubleHandedWeapon.CooldownComponent.CooldownSeconds = _normalCooldown;
    }

    [Command(requiresAuthority = false)]
    public void CmdUseSkill()
    {
        _isUseSkill = true;
    }

    [Command(requiresAuthority = false)]
    public void CmdStopUseSkill()
    {
        _isUseSkill = false;
    }

    public override void FinishCastPositive()
    {
        base.FinishCastPositive();
        CmdUseSkill();
        Invoke(nameof(CmdStopUseSkill), _buffDuration);
    }

    public void PowerUpSkillPoint(int points)
    {
        for (int i = 0; i < points; i++)
        {
            _buffDuration *= (1.1f - Level / 100);
            _damageModificator *= (1.1f - Level / 100);
            Level++;
        }
    }
}
