using Mirror.Examples.Tanks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] float meleeDamage = 200;
    [SerializeField] float rangeDamage = 50;
    [SerializeField] private GameObject projectile;

    public CustomTrigger leftAttackTrigger;
    public CustomTrigger rightAttackTrigger;
    public CustomTrigger centralAttackTrigger;
    public CustomTrigger sightTrigger;

    [SerializeField] private float rangeCooldownSec = 5;
    private CooldownComponent rangeCooldownComponent;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        rangeCooldownComponent = new CooldownComponent() { CooldownSeconds = rangeCooldownSec };
        rangeCooldownComponent.OnCooldownFinished += RangeCooldownComponent_OnCooldownFinished;

        leftAttackTrigger.EnteredTrigger += SetTargetInMeleeRange;
        rightAttackTrigger.EnteredTrigger += SetTargetInMeleeRange;
        centralAttackTrigger.EnteredTrigger += SetTargetInMeleeRange;

        sightTrigger.EnteredTrigger += OnSightTriggerEnter;
        sightTrigger.ExitedTrigger += OnSightTriggerExit;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        rangeCooldownComponent.HandleCooldown();
    }

    private void RangeCooldownComponent_OnCooldownFinished()
    {
        animator.SetBool("IsReady2Shoot", true);
    }

    private void SetTargetInMeleeRange(Collider2D collider)
    {
        var character = collider.GetComponent<Character>();
        if (character != null)
        {
            animator.SetBool("IsTargetInMeleeRange", true);
        }
    }

    private void OnSightTriggerEnter(Collider2D collision)
    {
        var character = collision.GetComponent<Character>();
        if (character != null)
        {
            animator.SetBool("IsInSight", true);
        }
    }

    private void OnSightTriggerExit(Collider2D collision)
    {
        var character = collision.GetComponent<Character>();
        if (character != null)
        {
            animator.SetBool("IsInSight", false);
        }
    }

    [Server]
    public void ShootProjectile()
    {
        var player = sightTrigger.colliderList.FirstOrDefault(c => c.CompareTag("Player"));
        if (player != null && rangeCooldownComponent.CanPerform)
        {
            var playerPosition = player.transform.position;
            var thisPosition = gameObject.transform.position;
            GameObject projectile2Spawn = Instantiate(this.projectile, thisPosition, gameObject.transform.rotation);
            var bulletComponent = projectile2Spawn.GetComponent<Projectile>();
            bulletComponent.Direction = (playerPosition - thisPosition).normalized;
            bulletComponent.Damage = rangeDamage;
            NetworkServer.Spawn(projectile2Spawn);
            rangeCooldownComponent.ResetCooldown();
            animator.SetBool("IsReady2Shoot", false);
        }
    }

    private void MeleeHit(CustomTrigger triggerBox)
    {
        var players = triggerBox.colliderList.Where(c => c.CompareTag("Player"));
        foreach (var player in players)
        {
            player.GetComponent<HealthComponent>().Damage(meleeDamage);
        }
    }

    public void LeftMeleeHit()
    {
        MeleeHit(leftAttackTrigger);
    }

    public void RightMeleeHit()
    {
        MeleeHit(rightAttackTrigger);
    }

    public void CentralMeleeHit()
    {
        MeleeHit(centralAttackTrigger);
    }
}
