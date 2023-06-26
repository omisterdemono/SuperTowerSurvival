using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttacker : NetworkBehaviour, IEnemyAttacker
{
    private IWeapon _weapon;
    private Transform _equipSlot;
    private Vector3 direction;

    public Transform Target
    {
        get; set;
    }

    public void AttackTarget()
    {
        Cmd_AttackFullCharge(direction);
    }

    [Command(requiresAuthority = false)]
    private void Cmd_AttackFullCharge(Vector2 direction)
    {
        _weapon.Attack();
    }


    void Start()
    {
        _weapon = GetComponentInChildren<IWeapon>();
        _equipSlot = transform.GetChild(0);
    }

    void Update()
    {
        if (Target == null)
        {
            return;
        }

        direction = (Target.position - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _equipSlot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
