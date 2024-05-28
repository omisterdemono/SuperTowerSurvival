using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileLauncher : NetworkBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject projectileRoot;
    [SerializeField] GameObject projectileTentacle;

    [SerializeField] GameObject SpiralProjectile;
    [SerializeField] GameObject SpiralProjectileRoot;
    [SerializeField] GameObject SpiralProjectileTentacle;

    [SerializeField] private GameObject tentacleObj;
    [SerializeField] private GameObject rootObj;
    [SerializeField] private StatusEffect _rootEffect;

    [Range(0f, 1f)]
    [SerializeField] float rootSpawnChance;

    [Range(0f, 1f)]
    [SerializeField] float tentacleSpawnChance;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Launch()
    {
        var projectile2Spawn = Instantiate(this.projectile, transform.position, Quaternion.Euler(0, 0, 0));
        var projectileComponent = projectile2Spawn.GetComponent<SpiralProjectile>();
        projectileComponent.Direction = Vector2.up;
        projectileComponent.Damage = 50;
        //projectile2Spawn.GetComponent<Projectile>().OnProjectileHit += Boss_OnTentacleProjectileHit;
        NetworkServer.Spawn(projectile2Spawn);
    }

    public void Launch(int number)
    {
        float angleStep = 360f / number;
        float angle = 0f;

        for (int i = 0; i < number; i++)
        {
            float projectileDirXPosition = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirYPosition = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 projectileDirection = new Vector2(projectileDirXPosition, projectileDirYPosition).normalized;

            var projectile2Spawn = Instantiate(this.projectile, transform.position, Quaternion.identity);
            var projectileComponent = projectile2Spawn.GetComponent<SpiralProjectile>();
            projectileComponent.Direction = projectileDirection;
            projectileComponent.AngularSpeed = -projectileComponent.AngularSpeed;
            projectileComponent.Damage = 50;
            //projectile2Spawn.GetComponent<Projectile>().OnProjectileHit += Boss_OnTentacleProjectileHit;
            NetworkServer.Spawn(projectile2Spawn);

            angle += angleStep;
        }
    }


    public void Launch(int number, GameObject projectile)
    {
        float angleStep = 360f / number;
        float angle = 0f;

        for (int i = 0; i < number; i++)
        {
            float projectileDirXPosition = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirYPosition = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 projectileDirection = new Vector2(projectileDirXPosition, projectileDirYPosition).normalized;

            var projectile2Spawn = Instantiate(this.projectile, transform.position, Quaternion.identity);
            var projectileComponent = projectile2Spawn.GetComponent<SpiralProjectile>();
            projectileComponent.Direction = projectileDirection;
            projectileComponent.AngularSpeed = -projectileComponent.AngularSpeed;
            projectileComponent.Damage = 50;
            //projectile2Spawn.GetComponent<Projectile>().OnProjectileHit += Boss_OnTentacleProjectileHit;
            NetworkServer.Spawn(projectile2Spawn);

            angle += angleStep;
        }
    }

    public void LaunchRandom(int number, bool isSpiral)
    {
        float angleStep = 360f / number;
        float angle = 0f;

        for (int i = 0; i < number; i++)
        {
            float projectileDirXPosition = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirYPosition = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 projectileDirection = new Vector2(projectileDirXPosition, projectileDirYPosition).normalized;

            //var projectile2Spawn = Instantiate(this.projectile, transform.position, Quaternion.identity);
            if (isSpiral)
            {
                var projectile2Spawn = ChooseSpiralProjectile(transform.position, Quaternion.identity);
                var projectileComponent = projectile2Spawn.GetComponent<SpiralProjectile>();
                projectileComponent.Direction = projectileDirection;
                projectileComponent.AngularSpeed = -projectileComponent.AngularSpeed;
                NetworkServer.Spawn(projectile2Spawn);
            }
            else
            {
                var projectile2Spawn = ChooseProjectile(transform.position, Quaternion.identity);
                var projectileComponent = projectile2Spawn.GetComponent<Projectile>();
                projectileComponent.Direction = projectileDirection;
                projectileComponent.Damage = 20;
                //projectileComponent.AngularSpeed = -projectileComponent.AngularSpeed;
                NetworkServer.Spawn(projectile2Spawn);
            }
            //var projectileComponent = projectile2Spawn.GetComponent<SpiralProjectile>();
            //projectile2Spawn.GetComponent<Projectile>().OnProjectileHit += Boss_OnTentacleProjectileHit;

            angle += angleStep;
        }
    }

    [Server]
    public  void PerformAttack()
    {
        int strategyIndex = Random.Range(0, 3);

        // Spiral Clockwise
        if (strategyIndex == 0)
        {
            LaunchRandom(6, true);
        }

        // Spiral counterclockwise
        else if (strategyIndex == 1)
        {
            LaunchRandom(6, true);
        }

        // Star-patern 
        else if (strategyIndex == 2)
        {
            LaunchRandom(12, false);
            
        }
    }

    private void PerformAttackSample()
    {
        //var playerPosition = player.transform.position;
        var thisPosition = gameObject.transform.position;

        //Vector2 directionToPlayer = (playerPosition - thisPosition).normalized;

        //float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        //Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject projectile2Spawn;
        var choice = Random.Range(0.0f, 1.0f);
        if (choice < rootSpawnChance)
        {
            //projectile2Spawn = Instantiate(this.projectileRoot, thisPosition, rotation);
            //projectile2Spawn.GetComponent<Projectile>().OnProjectileHit += Boss_OnRootProjectileHit;
        }
        else if (choice >= rootSpawnChance && choice < rootSpawnChance + tentacleSpawnChance)
        {
            //projectile2Spawn = Instantiate(this.projectileTentacle, thisPosition, rotation);
            //projectile2Spawn.GetComponent<Projectile>().OnProjectileHit += Boss_OnTentacleProjectileHit;

        }
        else
        {
            //projectile2Spawn = Instantiate(this.projectile, thisPosition, rotation);
        }

        //var bulletComponent = projectile2Spawn.GetComponent<Projectile>();
        //bulletComponent.Direction = directionToPlayer.normalized;
        //bulletComponent.Damage = rangeDamage;

        //NetworkServer.Spawn(projectile2Spawn);
    }

    [Server]
    private void Launcher_OnRootProjectileHit(Collider2D obj)
    {
        var pos = obj.transform.TransformPoint(rootObj.transform.position);
        GameObject root = Instantiate(rootObj, pos, Quaternion.Euler(Vector3.zero));
        var effectComponent = obj.GetComponent<EffectComponent>();
        if (effectComponent)
        {
            effectComponent.ApplyEffect(_rootEffect);
            effectComponent.OnEffectRemoved += () =>
            {
                if (isServer && root)
                {
                    StartCoroutine(Launcher_RemoveRoot(root));
                }
            };
        }

        NetworkServer.Spawn(root);
    }

    [Server]
    private IEnumerator Launcher_RemoveRoot(GameObject root)
    {
        var animator = root.GetComponent<Animator>();
        animator.SetTrigger("Despawn");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        NetworkServer.Destroy(root);
    }

    [Server]
    private void Launcher_OnTentacleProjectileHit(Collider2D obj)
    {
        var pos = obj.transform.position;
        GameObject tentacle = Instantiate(tentacleObj, pos, Quaternion.Euler(Vector3.zero));
        NetworkServer.Spawn(tentacle);
    }

    private GameObject ChooseProjectile(Vector3 position, Quaternion rotation)
    {
        var choice = Random.Range(0.0f, 1.0f);
        if (choice < rootSpawnChance)
        {
            return Instantiate(this.projectileRoot, position, rotation);
        }
        else if (choice >= rootSpawnChance && choice < rootSpawnChance + tentacleSpawnChance)
        {
            return Instantiate(this.projectileTentacle, position, rotation);

        }
        else
        {
            return Instantiate(this.projectile, position, rotation);
        }
    }

    private GameObject ChooseSpiralProjectile(Vector3 position, Quaternion rotation)
    {
        var choice = Random.Range(0.0f, 1.0f);
        if (choice < rootSpawnChance)
        {
            return Instantiate(SpiralProjectileRoot, position, rotation);
        }
        else if (choice >= rootSpawnChance && choice < rootSpawnChance + tentacleSpawnChance)
        {
            return Instantiate(SpiralProjectileTentacle, position, rotation);
        }
        else
        {
            return Instantiate(SpiralProjectile, position, rotation);
        }
    }
}
