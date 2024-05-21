using Mirror;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;

public class LifeInjectionSkill : ActiveSkill, ISkill
{
    private List<Collider2D> _playerColliders;
    private Collider2D _playerCollider;
    [SerializeField] private float _healModifier = 0.3f;
    [SerializeField] private float _selfHealModifier = 0.5f;
    [SyncVar(hook = "OnIsCasting")]
    private bool _isCasting = false;
    private ParticleSystem _particleSystem;

    private new void Start()
    {
        base.Start();
        _playerColliders = new List<Collider2D>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _playerCollider = GetComponent<CircleCollider2D>();
    }

    private void OnIsCasting(bool oldValue, bool newValue)
    {
        _isCasting = newValue;
        if (_isCasting)
        {
            _playerCollider.enabled = true;
            _particleSystem.Play();
            return;
        }
        _particleSystem.Stop();
        _playerCollider.enabled = false;
    }
    [Command(requiresAuthority = false)]
    private void CmdCasting()
    {
        _isCasting = !_isCasting;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision is BoxCollider2D)
            _playerColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision is BoxCollider2D)
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
        _playerCollider.enabled = false;
    }

    public override void FinishCastPositive()
    {
        GetComponent<CircleCollider2D>().enabled = true;
        CmdUseSkill();
        GetComponent<HealthComponent>().Heal(GetComponent<HealthComponent>().MaxHealth * _selfHealModifier);
        base.FinishCastPositive();
    }

    [Command(requiresAuthority = false)]
    void CmdUseSkill()
    {
        foreach (var players in _playerColliders)
        {
            players.GetComponent<Character>().IsAlive = true;
            players.GetComponent<Animator>().SetBool("IsAlive", true);
            players.GetComponent<HealthComponent>().Heal(players.GetComponent<HealthComponent>().MaxHealth * _healModifier);
        }
        GetComponent<CircleCollider2D>().enabled = false;
    }

    public void PowerUpSkillPoint(int points)
    {
        for (int i = 0; i < points; i++)
        {
            _healModifier *= (1.1f - Level / 100);
            _selfHealModifier *= (1.1f - Level / 100);
            Level++;
        }
    }
}
