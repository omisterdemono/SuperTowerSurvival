using Mirror.Examples.Tanks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components;
using UnityEngine;

public class Boss : NetworkBehaviour
{
    [SerializeField] float meleeDamage = 200;
    [SerializeField] float rangeDamage = 50;

    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject projectileRoot;
    [SerializeField] float rootSpawnChance = 0.1f;
    [SerializeField] private GameObject projectileTentacle;
    [SerializeField] float tenacleSpawnChance = 0.5f;
    [SerializeField] private GameObject tentacle;
    [SerializeField] private GameObject _deathEffect;
    [SerializeField] private GameObject _bossHealthBar;

    public CustomTrigger leftAttackTrigger;
    public CustomTrigger rightAttackTrigger;
    public CustomTrigger centralAttackTrigger;
    public CustomTrigger sightTrigger;

    [SerializeField] private float rangeCooldownSec = 5;
    private CooldownComponent rangeCooldownComponent;
    private HealthComponent healthComponent;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        rangeCooldownComponent = new CooldownComponent() { CooldownSeconds = rangeCooldownSec };
        healthComponent = GetComponent<HealthComponent>();
        //healthComponent.ChangeHealth(1000);
        
        healthComponent.OnDeath += PlayBossDeath;
        
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

    private void PlayBossDeath()
    {
        Cmd_Die(transform.position);
    }

    [Command(requiresAuthority = false)]
    private void Cmd_Die(Vector3 position)
    {
        GameObject explosion = Instantiate(_deathEffect, position, Quaternion.identity);
        NetworkServer.Spawn(explosion);
        NetworkServer.Destroy(this.gameObject);
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
            _bossHealthBar.SetActive(true);
            animator.SetBool("IsInSight", true);
        }
    }

    private void OnSightTriggerExit(Collider2D collision)
    {
        var character = collision.GetComponent<Character>();
        if (character != null)
        {
            _bossHealthBar.SetActive(false);
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

            Vector2 directionToPlayer = playerPosition - thisPosition;

            Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);

            GameObject projectile2Spawn;
            var choice = Random.Range(0.0f, 1.0f);
            if (choice < rootSpawnChance)
            {
                projectile2Spawn = Instantiate(this.projectileRoot, thisPosition, gameObject.transform.rotation);
            }
            else if (choice >= rootSpawnChance && choice < rootSpawnChance + tenacleSpawnChance)
            {
                projectile2Spawn = Instantiate(this.projectileTentacle, thisPosition, gameObject.transform.rotation);
                projectile2Spawn.GetComponent<Projectile>().OnProjectileHit += Boss_OnTentacleProjectileHit;
                
            }
            else
            {
                projectile2Spawn = Instantiate(this.projectile, thisPosition, gameObject.transform.rotation);
            }
            //GameObject projectile2Spawn = Instantiate(this.projectile, thisPosition, rotationToPlayer);

            var bulletComponent = projectile2Spawn.GetComponent<Projectile>();
            bulletComponent.Direction = directionToPlayer.normalized;
            bulletComponent.Damage = rangeDamage;

            NetworkServer.Spawn(projectile2Spawn);
            rangeCooldownComponent.ResetCooldown();
            animator.SetBool("IsReady2Shoot", false);
        }
    }

    [Server]
    private void Boss_OnTentacleProjectileHit(Collider2D obj)
    {
        var pos = obj.transform.position;
        GameObject tentacle = Instantiate(this.tentacle, pos, Quaternion.Euler(Vector3.zero));
        NetworkServer.Spawn(tentacle);

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
