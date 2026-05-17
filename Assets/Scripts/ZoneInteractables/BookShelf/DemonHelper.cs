using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ZoneInteractables;

public class DemonHelper : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float roamRadius = 20f;
    [SerializeField] private float waypointReachedDistance = 1f;

    [Header("Noise")]
    [SerializeField] private NoiseEmitter noiseEmitter;
    [SerializeField] private float noiseInterval = 2f;

    private NavMeshAgent _agent;
    private readonly List<Transform> _guardWaypoints = new List<Transform>();

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        CollectGuardWaypoints();
        SetNewDestination();
        StartCoroutine(EmitNoisePeriodically());
    }

    //Gather every waypoint from every Guard in the scene into one shared pool.
    private void CollectGuardWaypoints()
    {
        _guardWaypoints.Clear();
        foreach (BaseEnemy guard in FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None))
        {
            foreach (Transform waypoint in guard.waypoints)
            {
                if (waypoint != null)
                    _guardWaypoints.Add(waypoint);
            }
        }
    }

    private void Update()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= waypointReachedDistance)
            SetNewDestination();
    }

    private void SetNewDestination()
    {
        //Walk a Guard's patrol route; fall back to random roaming if no Guards exist.
        if (_guardWaypoints.Count > 0)
        {
            Transform target = _guardWaypoints[Random.Range(0, _guardWaypoints.Count)];
            _agent.SetDestination(target.position);
        }
        else if (RandomNavMeshPoint.TryGetRandomPoint(roamRadius, out Vector3 destination))
        {
            _agent.SetDestination(destination);
        }
    }

    private IEnumerator EmitNoisePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(noiseInterval);
            if (noiseEmitter != null)
                noiseEmitter.AlertNearbyEnemies();
        }
    }
}
