using Assets.Scripts.Weapons;
using Mirror;
using System;
using UnityEngine;

/// <summary>
/// Spawns bullet, rotates parent object for all weapons
/// towards cursor.
/// </summary>
public class FireWeapon : MonoBehaviour, IWeapon, IEquipable
{
    public enum Type
    {
        Rifle,
        Shotgun
    }


    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform[] _firePositions;
    [SerializeField] private float _damage;
    [SerializeField] private float _cooldownSeconds;
    [SerializeField] private bool _needFlip;
    [SerializeField] public Type type;

    public bool NeedRotation { get; set; } = true;

    public float Damage { get => _damage; set => _damage = value; }
    public bool NeedFlip { get => _needFlip; set => _needFlip = value; }
    //public Type Type { get => type; set => type = value; }
    public bool CanPerform => _cooldownComponent.CanPerform;
    public float CooldownSeconds => _cooldownSeconds;

    public Vector3 MousePosition { get; set; }

    public static event EventHandler OnShoot;

    private CooldownComponent _cooldownComponent;

    private void Awake()
    {
        _cooldownComponent = new CooldownComponent() { CooldownSeconds = _cooldownSeconds };
    }

    private void Update()
    {
        _cooldownComponent.HandleCooldown();
    }

    public void Attack()
    {
        if (!_cooldownComponent.CanPerform)
        {
            return;
        }
        _cooldownComponent.ResetCooldown();

        FireBullet();
    }

    public void FireBullet()
    {
        foreach (var firePosition in _firePositions)
        {
            GameObject projectile = Instantiate(_projectile, firePosition.position, firePosition.rotation);
            var bulletComponent = projectile.GetComponent<Projectile>();
            bulletComponent.Direction = firePosition.right;
            bulletComponent.Damage = _damage;
            NetworkServer.Spawn(projectile);
            OnShoot?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Hold()
    {
        Attack();
    }

    public void KeyUp()
    {
        return;
    }

    public void Interact()
    {
        Attack();
    }

    public void FinishHold()
    {
        KeyUp();
    }

    public void ChangeAnimationState()
    {
        return;
    }
}
