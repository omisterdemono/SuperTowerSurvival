using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceMineSkill : NetworkBehaviour, ISkill
{
    [SerializeField] private SkillAttributes _skillAttributes;
    [SerializeField] private GameObject _minePrefab;

    private ActiveSkill _activeSkill;

    public void Start()
    {
        _activeSkill = GetComponents<ActiveSkill>().Where(x => x.SkillName == _skillAttributes.Name).First();
        _activeSkill.Skill = this;
    }

    [Command(requiresAuthority = false)]
    public void UseSkill()
    {
        CmdUseSkill(GetComponent<Character>().transform.position);
    }

    public void CmdUseSkill(Vector2 position)
    {
        GameObject mine = Instantiate(_minePrefab, position, Quaternion.identity);
        mine.GetComponent<MineScript>().Damage = 10;
        NetworkServer.Spawn(mine, this.gameObject);
    }

    public void PowerUpSkillPoint()
    {
        throw new System.NotImplementedException();
    }
}
