using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    [Command(requiresAuthority = false)]
    public void UseSkill();
    public void PowerUpSkillPoint();
}
