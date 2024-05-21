using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPoisonousGasSkill : ActiveSkill, ISkill
{
    [SerializeField] private GameObject _poisonousGasPrefab;
    [SerializeField] private float _damage = 10;
    [SerializeField] private float _damageRate;
    [SerializeField] private float _time;
    [SerializeField] private float _slowDownModifier;

    public override void FinishCast()
    {
        base.FinishCast();
        CmdUseSkill(transform.position);
    }

    [Command(requiresAuthority = false)]
    public void CmdUseSkill(Vector2 throwFrom)
    {
        GameObject poisonousGas = Instantiate(_poisonousGasPrefab, throwFrom, Quaternion.identity);
        PoisonousGasScript poisonousGasScript = poisonousGas.GetComponent<PoisonousGasScript>();

        poisonousGasScript.Damage = _damage;
        poisonousGasScript.DamageRate = _damageRate;
        poisonousGasScript.WorkTime = _time;
        poisonousGasScript.SlowDownModifier = _slowDownModifier;
        poisonousGasScript.ThrowTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        NetworkServer.Spawn(poisonousGas, this.gameObject);
    }

    public void PowerUpSkillPoint(int points)
    {
        for (int i = 0; i < points; i++)
        {
            _damage *= (1.1f - Level / 100);
            _time *= (1.1f - Level / 100);
            _slowDownModifier *= (1.1f - Level / 100);
            Level++;
        }
    }
}
