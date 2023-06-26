using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilitySkill : ActiveSkill, ISkill
{
    [SyncVar(hook = "OnColorChanged")]
    Color color = new Color(1, 1, 1, 1);

    public void OnColorChanged(Color oldColor, Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
    }
    public void PowerUpSkillPoint()
    {
        throw new System.NotImplementedException();
    }

    public override void FinishCast()
    {
        base.FinishCast();
        CmdStopCasting();
    }

    [Command(requiresAuthority = false)]
    public void CmdCasting()
    {
        GetComponent<Character>().IsInvisible = true;
        color = new Color(1, 1, 1, 0.3f);
    }

    [Command(requiresAuthority = false)]
    public void CmdStopCasting()
    {
        GetComponent<Character>().IsInvisible = false;
        color = new Color(1, 1, 1, 1);
    }

    public override void StartCast()
    {
        base.StartCast();
        CmdCasting();
    }
}
