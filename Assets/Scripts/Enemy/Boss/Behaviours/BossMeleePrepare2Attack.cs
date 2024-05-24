using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components;
using Unity.Properties;
using UnityEngine;

public class BossMeleePrepare2Attack : StateMachineBehaviour
{
    private CustomTrigger leftAttackTrigger;
    private CustomTrigger rightAttackTrigger;
    private CustomTrigger centralAttackTrigger;

    private Animator animator;

    private void SetTriggeredMeleeBox(string boxName)
    {
        animator.SetBool("IsTargetInMeleeLeft", false);
        animator.SetBool("IsTargetInMeleeRight", false);
        animator.SetBool("IsTargetInMeleeCentral", false);
        animator.SetBool("IsTargetInMeleeRange", true);
        animator.SetBool(boxName, true);
    }

    private bool IsTriggerContainPlayer(CustomTrigger trigger) => trigger.colliderList.FirstOrDefault(c => c != null && c.CompareTag("Player")) != null;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        //var res = animator.GetComponentsInChildren<CustomTrigger>();
        //leftAttackTrigger = res[0];
        //rightAttackTrigger = res[1];
        //centralAttackTrigger = res[2];

        var boss = animator.GetComponent<Boss>();
        leftAttackTrigger = boss.leftAttackTrigger;
        rightAttackTrigger = boss.rightAttackTrigger;
        centralAttackTrigger = boss.centralAttackTrigger;

        if (IsTriggerContainPlayer(leftAttackTrigger))
        {
            SetTriggeredMeleeBox("IsTargetInMeleeLeft");
        }
        else if (IsTriggerContainPlayer(centralAttackTrigger))
        {
            SetTriggeredMeleeBox("IsTargetInMeleeCentral");
        }
        else if (IsTriggerContainPlayer(rightAttackTrigger))
        {
            SetTriggeredMeleeBox("IsTargetInMeleeRight");
        }
        else
        {

        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
