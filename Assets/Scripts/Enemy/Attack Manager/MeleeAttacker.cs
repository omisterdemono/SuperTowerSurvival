using Assets.Scripts.Weapons;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttacker : NetworkBehaviour, IEnemyAttacker
{
    private IEquipable _weapon;
    private Transform _equipSlot;
    private Vector3 direction;

    [SyncVar(hook = nameof(HandleEquipableAnimation))] private bool _isPerforming;
    private void HandleEquipableAnimation(bool oldValue, bool newValue)
    {
        _weapon.ChangeAnimationState();
    }

    public Transform Target 
    {
        get; set;
    }

    public void AttackTarget()
    {
        if (_weapon.CanPerform)
        {
            _isPerforming = true;
            _weapon.Interact();
            StartCoroutine(FinishAnimation());
        }
    }

    private IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(_weapon.CooldownSeconds);
        _isPerforming = false;
    }

    void Start()
    {
        _weapon = GetComponentInChildren<IEquipable>();
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
