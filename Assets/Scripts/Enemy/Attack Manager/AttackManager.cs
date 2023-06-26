using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AttackManager : NetworkBehaviour
{
    private Enemy _enemy;
    private HealthComponent _targetHealthComponent;
    private GameObject _target;
    //private GameObject _healthBar;

    private IEnemyAttacker _strategy;

    private bool attacking = false;
    private bool isAttackOnCooldown = false;
    private int damage;
    private float cooldown;
    private float timer;

    public void SetStrategy(IEnemyAttacker strategy)
    {
        _strategy = strategy;
    }

    public void SetTarget(Transform target)
    {
        _strategy.Target = target;
    }

    [Command (requiresAuthority = false)]
    public void AttackTarget()
    {
        if (_target == null) return;
        if (_targetHealthComponent == null) return;
        if (isAttackOnCooldown) return;
        //_targetHealthComponent.Damage(damage);
        //StartCoroutine(StartCooldown());
        _strategy.AttackTarget();
    }

    private IEnumerator StartCooldown()
    {
        // Set the cooldown flag
        isAttackOnCooldown = true;

        // Wait for one second
        yield return new WaitForSeconds(1f);

        // Reset the cooldown flag
        isAttackOnCooldown = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        //if (attacking)
        //{
        //    timer += Time.deltaTime;
        //    if (timer >= cooldown)
        //    {
        //        timer = 0;
        //        attacking = false;
        //    }
        //}
        //else
        //{
        //    AttackTarget();
        //    attacking = true;
        //}
    }

    public float GetTargetHealth()
    {
        if (_targetHealthComponent == null) return 0f;
        return _targetHealthComponent.CurrentHealth;
    }

    public void UpdateEnemy(Enemy newEnemy)
    {
        _enemy = newEnemy;
        damage = _enemy.damage;
        cooldown = _enemy.cooldown;
    }

    // Update is called once per frame
    public void UpdateTarget(GameObject newTarget)
    {
        _target = newTarget;
        _strategy.Target = newTarget.transform;
        //_healthBar = newTarget.GetComponent<Heal>
        _targetHealthComponent = _target.GetComponent<HealthComponent>();
    }
}
