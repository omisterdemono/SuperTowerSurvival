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

    public bool isTargetClosed;

    public bool useWallsStrategy;

    public GameObject Wall2Destroy = null;

    public Vector3 direction;

    public float nextWaypointDistance = 3;

    public bool reachedEndOfPath;

    //[SerializeField] private float _scanRadius = 7;

    [SerializeField] private float _distanceDiffKoef = 2;

    [SerializeField] private float _corelationDistancePerWall = 5;

    [SerializeField] private int _attackRadius = 3;

    private Path _path;

    private Seeker _seeker;

    private MovementComponent _movementComponent;

    private int currentWaypoint = 0;


    public void Start()
    {
        _movementComponent = GetComponent<MovementComponent>();
        _seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, .5f);
        AstarPath.active.logPathResults = PathLog.None;
    }

    public void UpdatePath()
    {
        _seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error)
        {
            _path = p;
            currentWaypoint = 0;

            isTargetClosed = target.position != _path.vectorPath.Last();
            //mb smooth in a future
        }
    }

    public void Update()
    {
        if (_path == null)
        {
            return;
        }

        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, _path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < _path.vectorPath.Count)
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

        List<RaycastHit2D> wallHits = RayCastWalls(target);
        Vector3 dir;
        if (IsBetterTroughWalls(wallHits))
        {
            useWallsStrategy = true;
            //here smt Sequence error
            Wall2Destroy = wallHits.First().collider.gameObject;
            var attackRadius = 1.5;
            if (DistanceToTarget(Wall2Destroy.transform) < _attackRadius)
            {
                reachedEndOfPath = true;
            }
            dir = (Wall2Destroy.transform.position - transform.position).normalized;
        }
        else
        {
            useWallsStrategy = false;
            Wall2Destroy = null;
            dir = (_path.vectorPath[currentWaypoint] - transform.position).normalized;
        }

        if (reachedEndOfPath)
        {
            var firstHit = wallHits.First();
            if (firstHit != null)
            {
                Wall2Destroy = firstHit.collider.gameObject;
                useWallsStrategy = true;
            }
            dir = Vector3.zero;
        }

        direction = dir;
    }

    private RaycastHit2D[] RayCast(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;

        float distance = Vector2.Distance(target.position, transform.position);

        return Physics2D.RaycastAll(transform.position, direction, distance);
    }
    private List<RaycastHit2D> RayCastWalls(Transform target)
    {
        RaycastHit2D[] hits = RayCast(target);
        return hits.Where(h => h.collider.gameObject.tag == "Wall").ToList();

        //List<RaycastHit2D> hitsWalls = new List<RaycastHit2D>();
        //hitsWalls = hits.Where(h => h.collider.gameObject.tag == "Wall").ToList();
        //foreach (RaycastHit2D hit in hits)
        //{
        //    if (hit.collider.gameObject.tag == "Wall")
        //    {
        //        hitsWalls.Add(hit);
        //    }
        //}
        //return hitsWalls;
    }

    public bool isTargetReachableThroughWall(Transform target)
    {
        var hits = RayCast(target);
        if (hits.Length == 0) return false;
        GameObject firstWall = hits.First().collider.gameObject;
        if (firstWall.tag == "Wall")
        {
            hits.Select(hit => hit.collider.gameObject.transform == target.transform).FirstOrDefault();
            if(hits.Length == 0)
            {
                return false;
            }
            else
            {
                //wall - target
                var dis = Vector3.Distance(firstWall.transform.position, target.position);
                if(dis < _attackRadius)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private float DistanceToTarget(Transform target)
    {
        var distance = Vector3.Distance(transform.position, target.position);
        return distance;
    }

    private bool IsBetterTroughWalls(List<RaycastHit2D> wallHits)
    {
        var wallCount = wallHits.Count();
        if (wallCount < 1) return false;

        var wallDistance = Vector3.Distance(transform.position, target.position) + _corelationDistancePerWall * wallCount;
        var pathDistance = _path.GetTotalLength();

        return wallDistance < pathDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, nextWaypointDistance);

        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(transform.position, _scanRadius);
    }
}