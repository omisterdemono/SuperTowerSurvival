using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBuilderSkill : ActiveSkill, ISkill
{
    [SerializeField] private float buffDuration = 30f;
    [SerializeField] private float repairModifier = 1.5f;
    [SerializeField] private float buildModifier = 1.5f;

    [SyncVar(hook = "OnUseSkill")]
    private bool _isUseSkill = false;

    private ParticleSystem _particleSystem;

    private void OnUseSkill(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            _particleSystem.Play();
            return;
        }
        _particleSystem.Stop();
    }

    private new void Start()
    {
        base.Start();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public override void FinishCast()
    {
        base.FinishCast();
    }

    public override void FinishCastPositive()
    {
        base.FinishCastPositive();
        CmdUseSkill();
        Invoke("CmdStopUseSkill", buffDuration);
    }

    [Command(requiresAuthority = false)]
    public void CmdUseSkill()
    {
        Character character = GetComponent<Character>();
        character.RepairSpeedModifier *= repairModifier;
        character.BuildSpeedModifier *= buildModifier;
        _isUseSkill = true;
    }

    [Command(requiresAuthority = false)]
    public void CmdStopUseSkill()
    {
        Character character = GetComponent<Character>();
        character.RepairSpeedModifier /= repairModifier;
        character.BuildSpeedModifier /= buildModifier;
        _isUseSkill = false;
    }


    public void PowerUpSkillPoint()
    {
        throw new System.NotImplementedException();
    }
}
