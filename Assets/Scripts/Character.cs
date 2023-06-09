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

    [SerializeField] private List<ActiveSkill> _activeSkills;
    [SerializeField] private List<PassiveSkill> _passiveSkills;

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

        foreach (var skill in _passiveSkills)
        {
            skill.UseSkill();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _activeSkills.First().UseSkill();
        }
    }

    public void PowerUpHealth()
    {
        //todo power up health
    }

    public void PowerUpSkill()
    {
        foreach (var skill in _activeSkills)
        {
            skill.PowerUpSkillPoint();
        }
        foreach (var skill in _passiveSkills)
        {
            skill.PowerUpSkillPoint();
        }
    }

    public void PowerUpWeapon()
    {
        //todo power up weapon
    }

}
