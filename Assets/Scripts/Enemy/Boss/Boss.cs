using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public CustomTrigger leftAttackTrigger;
    public CustomTrigger rightAttackTrigger;
    public CustomTrigger centralAttackTrigger;
    
    public CustomTrigger sightTrigger;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        //leftAttackTrigger.EnteredTrigger += OnLeftAttackTriggerEntered;
        //rightAttackTrigger.EnteredTrigger += OnRightAttackTriggerEntered;
        //centralAttackTrigger.EnteredTrigger += OnCentralAttackTriggerEntered;
        
        leftAttackTrigger.EnteredTrigger += SetTargetInMeleeRange;
        rightAttackTrigger.EnteredTrigger += SetTargetInMeleeRange;
        centralAttackTrigger.EnteredTrigger += SetTargetInMeleeRange;
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetTargetInMeleeRange(Collider2D collider)
    {
        var character = collider.GetComponent<Character>();
        if (character != null)
        {
            animator.SetBool("IsTargetInMeleeRange", true);
        }
    }

    private void SetMeleeBox(string boxName, bool state)
    {
        animator.SetBool("IsTargetInMeleeLeft", false);
        animator.SetBool("IsTargetInMeleeRight", false);
        animator.SetBool("IsTargetInMeleeCentral", false);
        animator.SetBool("IsTargetInMeleeRange", true);
        animator.SetBool(boxName, true);
        //animator.SetBool("IsFinishedMelee", false);

    }

    void OnLeftAttackTriggerEntered(Collider2D collider)
    {
        var character = collider.GetComponent<Character>();
        if (character != null)
        {
            animator.SetBool("IsTargetInMeleeRange", true);
            //SetMeleeBox("LeftAttack", true);
            //animator.SetTrigger("LeftAttack");
        }
    }

    void OnRightAttackTriggerEntered(Collider2D collider)
    {
        var character = collider.GetComponent<Character>();
        if (character != null )
        {
            animator.SetBool("IsTargetInMeleeRange", true);

            //SetMeleeBox("RightAttack", true);
            //animator.SetTrigger("RightAttack");
        }
    }

    void OnCentralAttackTriggerEntered(Collider2D collider)
    {
        var character = collider.GetComponent<Character>();
        if (character != null )
        {
            //animator.SetTrigger("InSight");
            //animator.SetBool("IsInSight", true);
            animator.SetBool("IsTargetInMeleeRange", true);

            //SetMeleeBox("FrontAttack", true);
            //animator.SetTrigger("FrontAttack");

            //Debug.Log("Player entered");
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.GetComponent<Character>();
        if (character != null)
        {
            //animator.SetTrigger("InSight");
            animator.SetBool("IsInSight", true);
            //Debug.Log("Player entered");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var character = collision.GetComponent<Character>();
        if (character != null)
        {
            //animator.SetTrigger("OutSight");
            animator.SetBool("IsInSight", false);
            //Debug.Log("Player entered");
        }
    }
}
