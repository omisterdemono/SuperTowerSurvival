using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfMovingTurretSkill : ActiveSkill, ISkill
{
    [SerializeField] private GameObject _turretPrefab;
    [SerializeField] private float _damage;
    [SerializeField] private float _fireCooldown;
    [SerializeField] private int maxTurretsCount;
    private List<GameObject> _turrets = new List<GameObject>();

    public List<GameObject> Turrets { get => _turrets; set => _turrets = value; }

    public override void FinishCastPositive()
    {
        base.FinishCastPositive();
        CmdUseSkill(transform.position);
    }

    [Command(requiresAuthority = false)]
    public void CmdUseSkill(Vector2 position)
    {
        GameObject turret = Instantiate(_turretPrefab, position, Quaternion.identity);
        turret.GetComponent<SelfMovingTurretScript>().Damage = _damage;
        turret.GetComponent<SelfMovingTurretScript>().CooldownSeconds = _fireCooldown;
        turret.GetComponent<SelfMovingTurretScript>().SetTarget(netId);
        _turrets.Add(turret);
        NetworkServer.Spawn(turret, this.gameObject);
    }

    protected override bool CanUseSkill()
    {
        if (_turrets.Count < maxTurretsCount)
        {
            return true;
        }
        return false;
    }

    public void PowerUpSkillPoint(int points)
    {
        throw new System.NotImplementedException();
    }
}
