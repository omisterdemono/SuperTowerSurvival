using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;

public class DoubleHandedWeaponSkill : ActiveSkill, ISkill
{
    [SerializeField]
    protected GameObject _mainWeapon;
    [SerializeField]
    protected GameObject _doubleHandedWeapon;
    [SerializeField] 
    private float _buffDuration = 30f;
    private Character _character;

    [SyncVar(hook = nameof(OnUseSkill))]
    private bool _isUseSkill = false;

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
        _mainWeapon.SetActive(false);
        _doubleHandedWeapon.SetActive(true);
        _character.CanScrollTools = false;
        _character.Equipables[0] = _doubleHandedWeapon.GetComponent<FireWeapon>();
    }

    private void Debuff()
    {
        _mainWeapon.SetActive(true);
        _doubleHandedWeapon.SetActive(false);
        _character.CanScrollTools = true;
        _character.Equipables[0] = _mainWeapon.GetComponent<FireWeapon>();
    }

    private new void Start()
    {
        base.Start();
        _character = GetComponent<Character>();
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

    public override void FinishCast()
    {
        base.FinishCast();
    }

    protected override bool CanUseSkill()
    {
        if (_character.EquipedSlot == 0)
        {
            return true;
        }
        return false;
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
