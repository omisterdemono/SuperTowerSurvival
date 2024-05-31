using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class InvisibilitySkill : ActiveSkill, ISkill
{
    [SyncVar(hook = "OnColorChanged")]
    Color color = new Color(1, 1, 1, 1);

    [SerializeField] float _buffDuration;

    [SerializeField] StatusEffect _speedBuff;

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
        GetComponent<Character>().IsInvisible = true;
        GetComponent<EffectComponent>().ApplyEffect(_speedBuff);
        color = new Color(1, 1, 1, 0.3f);
    }

    private void Debuff()
    {
        GetComponent<Character>().IsInvisible = false;
        color = new Color(1, 1, 1, 1);
    }

    [ClientRpc]
    public void OnColorChanged(Color oldColor, Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
    }
    public void PowerUpSkillPoint(int points)
    {
        throw new System.NotImplementedException();
    }

    public override void FinishCast()
    {
        base.FinishCast();
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
