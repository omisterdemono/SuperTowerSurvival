using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFinderOLD : MonoBehaviour
{
    public Transform target;

    public float speed = 2f;
    public float nextWeightPointDistance = 0.4f;

    private MovementComponent _movementComponent;

    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        _movementComponent = GetComponent<MovementComponent>();

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        //InvokeRepeating("UpdatePath", 0f, .5f);
        //seeker.StartPath(rb.position, target.position, OnPathComplete);
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    void UpdatePath()
    {
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    void Update()
    {
        if(path == null)
        {
            return;
        }
        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        //Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position);
        var direction = (path.vectorPath[currentWayPoint] - transform.position);
        //var direction = (path.vectorPath[1] - transform.position);

        _movementComponent.MovementVector = direction;
        _movementComponent.Move();
        //Vector2 force = direction * speed * Time.deltaTime;
        //rb.AddForce(force);

        //float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        float distance = Vector2.Distance((Vector2)transform.position, path.vectorPath[currentWayPoint]);
        Debug.Log($"Distance: {distance}");
        Debug.Log($"Vector Path: {path.vectorPath[currentWayPoint]}");

        if (distance < nextWeightPointDistance)
        {
            currentWayPoint++;
        }
    }
}
