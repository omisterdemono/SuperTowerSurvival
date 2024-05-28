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

    [SerializeField] int spiralPatternAmount = 6;
    [SerializeField] int spiralsAmount = 3;
    [SerializeField] float spiralsBetweenDelaySec = 0.1f;
    [SerializeField] int starPatternAmount = 12;

    [Range(0f, 1f)]
    [SerializeField] float rootSpawnChance;

    [Range(0f, 1f)]
    [SerializeField] float tentacleSpawnChance;

    public void LaunchRandomSpiral(int number, bool isClockWise)
    {
        float angleStep = 360f / number;
        float angle = 0f;

        for (int i = 0; i < number; i++)
        {
            float projectileDirXPosition = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirYPosition = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 projectileDirection = new Vector2(projectileDirXPosition, projectileDirYPosition).normalized;


            var projectile2Spawn = ChooseSpiralProjectile(transform.position, Quaternion.identity);
            var projectileComponent = projectile2Spawn.GetComponent<SpiralProjectile>();
            projectileComponent.Direction = projectileDirection;
            if(!isClockWise) projectileComponent.AngularSpeed = -projectileComponent.AngularSpeed;
            NetworkServer.Spawn(projectile2Spawn);

            angle += angleStep;
        }
    }

    public void LaunchRandomStar(int number)
    {
        float angleStep = 360f / number;
        float angle = 0f;

        for (int i = 0; i < number; i++)
        {
            float projectileDirXPosition = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirYPosition = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 projectileDirection = new Vector2(projectileDirXPosition, projectileDirYPosition).normalized;

            var projectile2Spawn = ChooseProjectile(transform.position, Quaternion.identity);
            var projectileComponent = projectile2Spawn.GetComponent<Projectile>();
            projectileComponent.Direction = projectileDirection;
            projectileComponent.Damage = 20;
            NetworkServer.Spawn(projectile2Spawn);

            angle += angleStep;
        }
    }

    [Server]
    public void PerformAttack()
    {
        StartCoroutine(PerformAttackCoroutine());
    }

    private IEnumerator PerformAttackCoroutine()
    {
        int strategyIndex = Random.Range(0, 3);

        // Spiral Clockwise
        if (strategyIndex == 0)
        {
            for (int i = 0; i < spiralsAmount; i++)
            {
                LaunchRandomSpiral(spiralPatternAmount, true);
                yield return new WaitForSeconds(spiralsBetweenDelaySec);
            }
        }

        // Spiral counterclockwise
        else if (strategyIndex == 1)
        {
            for (int i = 0; i < spiralsAmount; i++)
            {
                LaunchRandomSpiral(spiralPatternAmount, false);
                yield return new WaitForSeconds(spiralsBetweenDelaySec);
            }
        }

        // Star-patern 
        else if (strategyIndex == 2)
        {
            LaunchRandomStar(starPatternAmount);
        }

        yield return null;
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
        GameObject projectile;
        var choice = Random.Range(0.0f, 1.0f);
        // spawn root
        if (choice < rootSpawnChance)
        {
            projectile = Instantiate(projectileRoot, position, rotation);
            projectile.GetComponent<Projectile>().OnProjectileHit += Launcher_OnRootProjectileHit;
        }
        // spawn tentacle
        else if (choice >= rootSpawnChance && choice < rootSpawnChance + tentacleSpawnChance)
        {
            projectile = Instantiate(projectileTentacle, position, rotation);
            projectile.GetComponent<Projectile>().OnProjectileHit += Launcher_OnTentacleProjectileHit;

        }
        else
        {
            projectile = Instantiate(this.projectile, position, rotation);
        }
        return projectile;
    }

    private GameObject ChooseSpiralProjectile(Vector3 position, Quaternion rotation)
    {
        GameObject projectile;
        var choice = Random.Range(0.0f, 1.0f);
        if (choice < rootSpawnChance)
        {
            projectile = Instantiate(SpiralProjectileRoot, position, rotation);
            projectile.GetComponent<SpiralProjectile>().OnProjectileHit += Launcher_OnRootProjectileHit;
        }
        else if (choice >= rootSpawnChance && choice < rootSpawnChance + tentacleSpawnChance)
        {
            projectile = Instantiate(SpiralProjectileTentacle, position, rotation);
            projectile.GetComponent<SpiralProjectile>().OnProjectileHit += Launcher_OnTentacleProjectileHit;
        }
        else
        {
            projectile = Instantiate(SpiralProjectile, position, rotation);
        }
        return projectile;
    }
}
