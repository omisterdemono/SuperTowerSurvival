using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAttacker
{
    Transform Target { get; set; }
    void AttackTarget();
}
