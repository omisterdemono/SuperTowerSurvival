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
    [SerializeField]
    private float _buffDuration = 30f;
    [SyncVar(hook = nameof(OnUseSkill))]
    private bool _isUseSkill = false;
    private Character _character;

    private new void Start()
    {
        base.Start();
        _character = GetComponent<Character>();
    }

    private void OnUseSkill(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            _mainWeapon.Projectile = _redProjectile;
            _doubleHandedWeapon.Projectile = _redProjectile;
            _mainWeapon.CooldownSeconds = 0.1f;
            _doubleHandedWeapon.CooldownSeconds = 0.1f;
            _mainWeapon.Damage *= _damageModificator;
            _doubleHandedWeapon.Damage *= _damageModificator;
            return;
        }
        _mainWeapon.Projectile = _projectile;
        _doubleHandedWeapon.Projectile = _projectile;
        _mainWeapon.CooldownSeconds = 0.3f;
        _doubleHandedWeapon.CooldownSeconds = 0.3f;
        _mainWeapon.Damage /= _damageModificator;
        _doubleHandedWeapon.Damage /= _damageModificator;
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

    public void PowerUpSkillPoint()
    {
        throw new System.NotImplementedException();
    }
}
