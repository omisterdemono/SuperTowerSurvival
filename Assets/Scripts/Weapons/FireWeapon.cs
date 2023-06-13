using Mirror;
using UnityEngine;

/// <summary>
/// Spawns bullet, rotates parent object for all weapons
/// towards cursor.
/// </summary>
public class FireWeapon : NetworkBehaviour, IAttacker
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float rotateSpeed = 1.0f;
    [SerializeField] private float rotateAngle = 1.0f;
    
    private SpriteRenderer _spriteRenderer;
    private Vector3 _direction;

    public float Damage { get; set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {


    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Rotate(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        _spriteRenderer.flipY = !(angle <= 90.0f && angle >= -90.0f);
    }
}
