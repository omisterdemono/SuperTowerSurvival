using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ObserveSkill : ActiveSkill, ISkill
{
    [SerializeField] float _buffDuration;
    [SerializeField] float _sightMod = 1.5f;

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
        GetComponent<Character>().sightDistance *= _sightMod;
    }

    private void Debuff()
    {
        GetComponent<Character>().sightDistance /= _sightMod;
    }

    public void PowerUpSkillPoint(int points)
    {
        throw new System.NotImplementedException();
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
}
