using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceDynamiteSkill : ActiveSkill, ISkill
{
    [SerializeField] private GameObject _dynamitePrefab;

    [SerializeField] private float _damage = 10;

    public override void FinishCastPositive()
    {
        base.FinishCastPositive();
        CmdUseSkill(transform.position);
    }

    [Command(requiresAuthority = false)]
    public void CmdUseSkill(Vector2 position)
    {
        GameObject mine = Instantiate(_dynamitePrefab, position, Quaternion.identity);
        mine.GetComponent<DynamiteScript>().Damage = _damage;
        NetworkServer.Spawn(mine, this.gameObject);
    }
    public void PowerUpSkillPoint()
    {
        throw new System.NotImplementedException();
    }
}
