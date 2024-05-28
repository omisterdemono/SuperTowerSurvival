using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components;
using UnityEngine;

public class Tentacle : NetworkBehaviour
{
    [SerializeField] float damage = 50;
    public CustomTrigger damageTrigger;

    [SerializeField] private float attackCooldownSec = 5;
    private CooldownComponent attackCooldownComponent;

    [SerializeField] private float time2LiveSec = 10;
    private CooldownComponent time2LiveCooldownComponent;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        attackCooldownComponent = new CooldownComponent() { CooldownSeconds = attackCooldownSec };
        attackCooldownComponent.OnCooldownFinished += CooldownComponent_OnCooldownFinished;

        time2LiveCooldownComponent = new CooldownComponent() { CooldownSeconds = time2LiveSec };
        time2LiveCooldownComponent.OnCooldownFinished += Time2LiveCooldownComponent_OnCooldownFinished;
        time2LiveCooldownComponent.ResetCooldown();

        damageTrigger.EnteredTrigger += DamageTrigger_EnteredTrigger;
        damageTrigger.ExitedTrigger += DamageTrigger_ExitedTrigger;

    }

    private void DamageTrigger_EnteredTrigger(Collider2D obj)
    {
        var character = obj.GetComponent<Character>();
        if (character != null && character.IsAlive)
        {
            animator.SetBool("IsTargetInAttackRange", true);
        }
    }

    private void DamageTrigger_ExitedTrigger(Collider2D obj)
    {
        var character = obj.GetComponent<Character>();
        if (character != null)
        {
            animator.SetBool("IsTargetInAttackRange", false);
        }
    }

    private void CooldownComponent_OnCooldownFinished()
    {
        animator.SetBool("IsTargetInAttackRange", CheckIsAlivePlayerInAttackRange());
        animator.SetBool("IsReady2Attack", true);
    }

    private bool CheckIsAlivePlayerInAttackRange()
    {
        foreach (var collider in damageTrigger.colliderList)
        {
            var character = collider.GetComponent<Character>();
            if (character != null && character.IsAlive)
            {
                return true;
            }
        }
        return false; 
    }

    private void Time2LiveCooldownComponent_OnCooldownFinished()
    {
        animator.SetBool("IsActive", false);
    }

    void Start()
    {
        StartCoroutine(DestroyAfterTTL());
    }

    void Update()
    {
        time2LiveCooldownComponent.HandleCooldown();
        attackCooldownComponent.HandleCooldown();
    }

    public void Attack()
    {
        var players = damageTrigger.colliderList.Where(c => c.CompareTag("Player"));
        foreach (var player in players)
        {
            //if (player.GetComponent<Character>().IsAlive == false) break;
            player.GetComponent<HealthComponent>().Damage(damage);
        }
        attackCooldownComponent.ResetCooldown();
        animator.SetBool("IsReady2Attack", false);
    }

    private IEnumerator DestroyAfterTTL()
    {
        yield return new WaitForSeconds(time2LiveSec + 2);

        DestroyTentacle();
    }

    [Command(requiresAuthority = false)]
    private void DestroyTentacle()
    {
        NetworkServer.Destroy(gameObject);
    }
}
