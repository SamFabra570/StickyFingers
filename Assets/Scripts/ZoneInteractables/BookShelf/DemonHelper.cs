using System.Collections;
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

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
        StartCoroutine(EmitNoisePeriodically());
    }

    private void Update()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= waypointReachedDistance)
            SetNewDestination();
    }

    private void SetNewDestination()
    {
        if (RandomNavMeshPoint.TryGetRandomPoint(roamRadius, out Vector3 destination))
            _agent.SetDestination(destination);
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
