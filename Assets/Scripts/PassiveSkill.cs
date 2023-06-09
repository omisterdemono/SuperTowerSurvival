using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : NetworkBehaviour, ISkill
{
    GameObject player;

    public abstract void PowerUpSkillPoint();

    public abstract void UseSkill();
}
