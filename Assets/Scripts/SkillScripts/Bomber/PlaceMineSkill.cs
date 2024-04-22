using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceMineSkill : ActiveSkill, ISkill
{
    [SerializeField] private GameObject _minePrefab;
    [SerializeField] private float _damage = 10;

    public override void FinishCastPositive()
    {
        base.FinishCastPositive();
        CmdUseSkill(transform.position);
    }

    [Command(requiresAuthority = false)]
    public void CmdUseSkill(Vector2 position)
    {
        GameObject mine = Instantiate(_minePrefab, position, Quaternion.identity);
        mine.GetComponent<MineScript>().Damage = _damage;
        NetworkServer.Spawn(mine, this.gameObject);
    }

    public void PowerUpSkillPoint(int points)
    {
        throw new System.NotImplementedException();
    }
}
