using System;
using UnityEngine;

public interface IAttacker
{
    float Damage { get; set; }   
    void Attack(Vector2 direction, ref Action<Vector2> performAttack);
}
