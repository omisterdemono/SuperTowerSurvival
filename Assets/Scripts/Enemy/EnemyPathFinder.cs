using UnityEngine;
using Pathfinding;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;

public class EnemyPathFinder : MonoBehaviour
{
    public Transform target;

    //public bool isTargetClosen;

    public bool isTargetClosen;

    public bool useWallsStrategy;

    public Path path;

    public float speed = 2;

    public float nextWaypointDistance = 3;

    public bool reachedEndOfPath;

    public GameObject Wall2Destroy = null;

    [SerializeField] private float _scanRadius = 7;

    [SerializeField] private float _distanceDiffKoef = 2;

    [SerializeField] private float corelationDistancePerWall = 5;

    private Seeker _seeker;

    private MovementComponent _movementComponent;

    private Vector3 _previousPosition;

    private int currentWaypoint = 0;

    public void Start()
    {
        _movementComponent = GetComponent<MovementComponent>();
        //_enemy = GetComponent<Enemy>();
        _seeker = GetComponent<Seeker>();
        var res = RayCast(target);
        //_seeker.
        InvokeRepeating("UpdatePath", 0f, .5f);
        InvokeRepeating("SaveLastPosition", 0f, 1.0f);
    }

    void SaveLastPosition()
    {
        _previousPosition = transform.position;
    }

    void UpdatePath()
    {
        _seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;

            isTargetClosen = target.position != path.vectorPath.Last();
            //mb smooth in a future
        }
    }

    public void Update()
    {
        if (path == null)
        {
            return;
        }

        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }
        }
        List<RaycastHit2D> hits = RayCast(target);
        Vector3 dir;
        if ((IsBetterTroughWalls(hits) || isTargetClosen) && hits.Count() != 0)
        {
            useWallsStrategy = true;
            Wall2Destroy = hits.First().collider.gameObject;
            var attackRadius = 1.5;
            if (DistanceToTarget(Wall2Destroy.transform) < attackRadius)
            {
                reachedEndOfPath = true;
            }
            dir = (Wall2Destroy.transform.position - transform.position).normalized;
        }
        else
        {
            useWallsStrategy = false;
            Wall2Destroy = null;
            dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        }

        if (reachedEndOfPath)
        {
            dir = Vector3.zero;
        }

        _movementComponent.MovementVector = dir;
        _movementComponent.Move();
    }

    private List<RaycastHit2D> RayCast(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;

        float distance = Vector2.Distance(target.position, transform.position);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, distance);

        List<RaycastHit2D> hitsWalls = new List<RaycastHit2D>();
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.tag == "Wall")
            {
                hitsWalls.Add(hit);
            }
        }
        return hitsWalls;
    }

    private float DistanceToTarget(Transform target)
    {
        var distance = Vector3.Distance(transform.position, target.position);
        return distance;
    }

    private bool IsBetterTroughWalls(List<RaycastHit2D> wallHits)
    {
        var wallCount = wallHits.Count();

        var wallDistance = Vector3.Distance(transform.position, target.position) + corelationDistancePerWall * wallCount;
        var pathDistance = path.GetTotalLength();

        return wallDistance < pathDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, nextWaypointDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _scanRadius);
    }
}