using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeInjectionSkill : ActiveSkill, ISkill
{
    private List<Collider2D> _playerColliders;
    [SerializeField] private float _healModifier = 0.3f;
    [SerializeField] private float _selfHealModifier = 0.5f;
    [SyncVar(hook = "OnIsCasting")]
    private bool _isCasting = false;
    private ParticleSystem _particleSystem;

    private void Start()
    {
        base.Start();
        _playerColliders = new List<Collider2D>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnIsCasting(bool oldValue, bool newValue)
    {
        _isCasting = newValue;
        if (_isCasting)
        {
            _particleSystem.Play();
            return;
        }
        _particleSystem.Stop();
    }
    [Command(requiresAuthority = false)]
    private void CmdCasting()
    {
        _isCasting = !_isCasting;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _playerColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _playerColliders.Remove(collision);
    }

    public override void StartCast()
    {
        base.StartCast();
        CmdCasting();
    }

    public override void FinishCast()
    {
        base.FinishCast();
        GetComponentInChildren<ParticleSystem>().Stop();
        CmdCasting();
    }

    public override void FinishCastPositive()
    {
        base.FinishCastPositive();
        GetComponent<CircleCollider2D>().enabled = true;
        CmdUseSkill();
        GetComponent<HealthComponent>().Heal(GetComponent<HealthComponent>().MaxHealth * _selfHealModifier);
    }

    [Command(requiresAuthority = false)]
    void CmdUseSkill()
    {
        foreach (var players in _playerColliders)
        {
            players.GetComponent<Character>().IsAlive = true;
            players.GetComponent<HealthComponent>().Heal(players.GetComponent<HealthComponent>().MaxHealth * _healModifier);
        }
        GetComponent<CircleCollider2D>().enabled = false;
    }

    public void PowerUpSkillPoint()
    {
        throw new System.NotImplementedException();
    }
}
