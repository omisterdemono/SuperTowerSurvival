using System;
using UnityEngine;

public interface IAttacker
{
    float Damage { get; set; }   
    void Attack(Vector2 direction);
    void Hold(Vector2 direction);
    void KeyUp(Vector2 direction);
}
