using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttacker : MonoBehaviour, IEnemyAttacker
{
    private IWeapon _weapon;
    private Transform _equipSlot;

    private Vector3 direction;
    //private Enemy _enemy;

    public Transform Target 
    {
        get; set;
    }

    public void AttackTarget()
    {
        _weapon.Attack(direction);
    }

    void Start()
    {
        _weapon = GetComponentInChildren<IWeapon>();
        _equipSlot = transform.GetChild(0);
    }

    void Update()
    {
        if(Target == null) 
        {
            return; 
        }

        direction = (Target.position - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _equipSlot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
