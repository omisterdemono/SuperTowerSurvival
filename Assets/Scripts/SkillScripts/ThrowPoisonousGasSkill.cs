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
        poisonousGas.GetComponent<PoisonousGasScript>().Damage = _damage;
        poisonousGas.GetComponent<PoisonousGasScript>().DamageRate = _damageRate;
        poisonousGas.GetComponent<PoisonousGasScript>().WorkTime = _time;
        poisonousGas.GetComponent<PoisonousGasScript>().SlowDownModifier = _slowDownModifier;
        poisonousGas.GetComponent<PoisonousGasScript>().ThrowTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        NetworkServer.Spawn(poisonousGas, this.gameObject);
    }

    public void PowerUpSkillPoint()
    {
        throw new System.NotImplementedException();
    }
}
