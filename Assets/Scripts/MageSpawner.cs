using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
public class MageSpawner : MonoBehaviour
{
    public static  MageSpawner Instance;

    public Transform mageSpawn;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject mage;

    //Base patrol authored in the scene — the mage roams the WHOLE level over these, plus the detection
    //clusters added on top of them (see SpawnMage).
    public List<Transform> waypoints;

    [Tooltip("How far from the spotted player the mage appears. NavMesh-sampled, so it never lands on top of the player.")]
    public float spawnDistanceFromPlayer = 9f;
    [Tooltip("Radius of the patrol cluster scattered around each detection point.")]
    public float detectionClusterRadius = 6f;
    [Tooltip("How many patrol points each scout detection adds around the spotted position.")]
    public int detectionClusterCount = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Called by a scout the instant it spots the player, passing WHERE it saw them.
    public void SpawnMage(Vector3 detectionPoint)
    {
        // Only one mage may exist at a time (task 3). Additional scouts do NOT stack a second mage —
        // instead they pile a fresh patrol hot-zone around their own detection point onto the existing one.
        if (mage != null)
        {
            mage.GetComponent<BaseMageEnemy>().AddPatrolArea(detectionPoint, detectionClusterRadius, detectionClusterCount);
            return;
        }

        GameObject prefab = Resources.Load<GameObject>("Mage");
        mage = Instantiate(prefab);

        BaseMageEnemy mageEnemy = mage.GetComponent<BaseMageEnemy>();

        // Seed the base patrol from the scene waypoints so the mage roams the whole level...
        mageEnemy.patrolPoints = new List<Vector3>();
        if (waypoints != null)
        {
            foreach (Transform wp in waypoints)
                if (wp != null) mageEnemy.patrolPoints.Add(wp.position);
        }

        // ...then focus a cluster of patrol points around where the player was actually spotted.
        mageEnemy.AddPatrolArea(detectionPoint, detectionClusterRadius, detectionClusterCount);

        // Appear NEAR the detection point (not on top of the player), falling back to the fixed spawn.
        mage.transform.position = ResolveSpawnPosition(detectionPoint);

        UIManager.Instance.ShowMageSpawnNotif();
    }

    // Picks a NavMesh-valid point roughly spawnDistanceFromPlayer away from the detection point.
    private Vector3 ResolveSpawnPosition(Vector3 detectionPoint)
    {
        Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 candidate = detectionPoint + new Vector3(dir.x, 0f, dir.y) * spawnDistanceFromPlayer;

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, spawnDistanceFromPlayer, NavMesh.AllAreas))
            return hit.position;

        return mageSpawn != null ? mageSpawn.position : detectionPoint;
    }
}
