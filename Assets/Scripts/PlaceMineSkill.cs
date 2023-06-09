using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMineSkill : ActiveSkill
{
    [SerializeField] private GameObject _minePrefab;

    public override void UseSkill()
    {
        base.UseSkill();

        Cmd_PlaceMine(Vector3.zero);
    }

    [Command]
    public void Cmd_PlaceMine(Vector3 position)
    {
        Rpc_PlaceMine(position);
    }

    [ClientRpc]
    public void Rpc_PlaceMine(Vector3 position)
    {
        GameObject mine = Instantiate(_minePrefab, position, Quaternion.identity);
        NetworkServer.Spawn(mine);
    }

    public override void StartCast()
    {
        base.StartCast();
    }

    public override void FinishCast()
    {
        base.FinishCast();
    }

    public override void PowerUpSkillPoint()
    {
        throw new System.NotImplementedException();
    }
}
