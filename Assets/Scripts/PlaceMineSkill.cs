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
        if (!isOwned) return;
        GameObject mine = Instantiate(_minePrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(mine);
    }

    public void PowerUpSkillPoint()
    {
        throw new System.NotImplementedException();
    }
}
