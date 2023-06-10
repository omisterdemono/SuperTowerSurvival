using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using System.Linq;

public class Character : NetworkBehaviour
{
    private MovementComponent _movement;
    private HealthComponent _health;

    [SerializeField] private float _repairSpeedModifier = 1;
    [SerializeField] private float _buildSpeedModifier = 1;
    [SerializeField] private float _weaponDamageModifier = 1;

    [SerializeField] private List<ActiveSkill> _activeSkill;

    [SerializeField] private GameObject _minePrefab;

    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
    }

    void FixedUpdate()
    {
        if (!isOwned) return;

        Vector3 moveVector = Vector3.zero;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        moveVector.x = inputX;
        moveVector.y = inputY;

        _movement.MovementVector = moveVector;
        _movement.Move();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _activeSkill[0].IsReady = true;
            
        }
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    _activeSkill[0].GetComponent<ISkill>().UseSkill();
        //}
    }

    public void PowerUpHealth()
    {
        //todo power up health
    }

    public void PowerUpSkill()
    {
        
    }

    public void PowerUpWeapon()
    {
        //todo power up weapon
    }

}
