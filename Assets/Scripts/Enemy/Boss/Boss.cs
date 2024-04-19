using Mirror.Examples.Tanks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] float meleeDamage = 200;
    [SerializeField] float rangeDamage = 50;
    [SerializeField] private GameObject projectile;

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

        sightTrigger.EnteredTrigger += OnSightTriggerEnter;
        sightTrigger.ExitedTrigger += OnSightTriggerExit;
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

    //private void SetMeleeBox(string boxName, bool state)
    //{
    //    animator.SetBool("IsTargetInMeleeLeft", false);
    //    animator.SetBool("IsTargetInMeleeRight", false);
    //    animator.SetBool("IsTargetInMeleeCentral", false);
    //    animator.SetBool("IsTargetInMeleeRange", true);
    //    animator.SetBool(boxName, true);
    //    //animator.SetBool("IsFinishedMelee", false);

    //}

    //void OnLeftAttackTriggerEntered(Collider2D collider)
    //{
    //    var character = collider.GetComponent<Character>();
    //    if (character != null)
    //    {
    //        animator.SetBool("IsTargetInMeleeRange", true);
    //        //SetMeleeBox("LeftAttack", true);
    //        //animator.SetTrigger("LeftAttack");
    //    }
    //}

    //void OnRightAttackTriggerEntered(Collider2D collider)
    //{
    //    var character = collider.GetComponent<Character>();
    //    if (character != null )
    //    {
    //        animator.SetBool("IsTargetInMeleeRange", true);

    //        //SetMeleeBox("RightAttack", true);
    //        //animator.SetTrigger("RightAttack");
    //    }
    //}

    //void OnCentralAttackTriggerEntered(Collider2D collider)
    //{
    //    var character = collider.GetComponent<Character>();
    //    if (character != null )
    //    {
    //        //animator.SetTrigger("InSight");
    //        //animator.SetBool("IsInSight", true);
    //        animator.SetBool("IsTargetInMeleeRange", true);

    //        //SetMeleeBox("FrontAttack", true);
    //        //animator.SetTrigger("FrontAttack");

    //        //Debug.Log("Player entered");
    //    }

    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    var character = collision.GetComponent<Character>();
    //    if (character != null)
    //    {
    //        //animator.SetTrigger("InSight");
    //        animator.SetBool("IsInSight", true);
    //        //Debug.Log("Player entered");
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    var character = collision.GetComponent<Character>();
    //    if (character != null)
    //    {
    //        //animator.SetTrigger("OutSight");
    //        animator.SetBool("IsInSight", false);
    //        //Debug.Log("Player entered");
    //    }
    //}

    private void OnSightTriggerEnter(Collider2D collision)
    {
        var character = collision.GetComponent<Character>();
        if (character != null)
        {
            animator.SetBool("IsInSight", true);
        }
    }

    private void OnSightTriggerExit(Collider2D collision)
    {
        var character = collision.GetComponent<Character>();
        if (character != null)
        {
            animator.SetBool("IsInSight", false);
        }
    }

    [Server]
    public void ShootProjectile()
    {
        var player = sightTrigger.colliderList.FirstOrDefault(c => c.CompareTag("Player"));
        if (player != null)
        {
            var playerPosition = player.transform.position;
            var thisPosition = gameObject.transform.position;
            GameObject projectile2Spawn = Instantiate(this.projectile, thisPosition, gameObject.transform.rotation);
            var bulletComponent = projectile2Spawn.GetComponent<Projectile>();
            bulletComponent.Direction = (playerPosition - thisPosition).normalized;
            bulletComponent.Damage = rangeDamage;
            NetworkServer.Spawn(projectile2Spawn);
        }
    }

    private void MeleeHit(CustomTrigger triggerBox)
    {
        var players = triggerBox.colliderList.Where(c => c.CompareTag("Player"));
        foreach (var player in players)
        {
            player.GetComponent<HealthComponent>().Damage(meleeDamage);
        }
    }

    public void LeftMeleeHit()
    {
        MeleeHit(leftAttackTrigger);
    }

    public void RightMeleeHit()
    {
        MeleeHit(rightAttackTrigger);
    }

    public void CentralMeleeHit()
    {
        MeleeHit(centralAttackTrigger);
    }
}
