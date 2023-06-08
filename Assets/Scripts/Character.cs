using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class Character : NetworkBehaviour
{
    private MovementComponent _movement;

    [SerializeField] float _repairSpeedModifier = 1;
    [SerializeField] float _buildSpeedModifier = 1;
    [SerializeField] float _weaponDamageModifier = 1;

    [SerializeField] List<ISkill> _activeSkills;
    [SerializeField] List<ISkill> _passiveSkills;

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
