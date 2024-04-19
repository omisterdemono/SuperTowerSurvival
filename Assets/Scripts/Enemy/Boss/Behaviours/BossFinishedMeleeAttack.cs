using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossFinishedMeleeAttack : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsTargetInMeleeLeft", false);
        animator.SetBool("IsTargetInMeleeRight", false);
        animator.SetBool("IsTargetInMeleeCentral", false);

        var triggers = animator.GetComponentsInChildren<CustomTrigger>();
        var boss = animator.GetComponent<Boss>();
        var allMeleeTriggers = boss.leftAttackTrigger.colliderList
            .Concat(boss.rightAttackTrigger.colliderList)
            .Concat(boss.centralAttackTrigger.colliderList)
            .ToArray();

        var player = allMeleeTriggers.FirstOrDefault(c => c.CompareTag("Player"));
        if(player != null)
        {
            animator.SetBool("IsTargetInMeleeRange", true);
            return;
        }
        //foreach (var trigger in triggers)
        //{
        //    if (trigger.colliderList.FirstOrDefault(c => c.CompareTag("Player")) != null)
        //    {
        //        animator.SetBool("IsTargetInMeleeRange", true);
        //        return;
        //    }
        //}
        animator.SetBool("IsTargetInMeleeRange", false);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
