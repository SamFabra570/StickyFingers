using UnityEngine;
using UnityEngine.AI;

public static class RandomNavMeshPoint
{
    public static bool TryGetRandomPoint(float range, out Vector3 result, int maxAttempts = 30)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPoint = Random.insideUnitSphere * range;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}