using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseMageEnemy : EnemyBrain
{
    //Patrol points are generated at runtime (base scene waypoints + detection clusters), so they are
    //world positions rather than Transforms. Seeded by MageSpawner.SpawnMage, grown by AddPatrolArea.
    public List<Vector3> patrolPoints = new List<Vector3>();
    [HideInInspector] public Vector3 currentTarget;
    [HideInInspector] public bool hasTarget = false;

    //Dynamic patrol: each scout detection scatters this many NavMesh-sampled points within this radius
    //around where the player was spotted, so the mage concentrates its patrol on hot zones.
    public float detectionClusterRadius = 6f;
    public int detectionClusterCount = 4;

    public override int PatrolPointCount => patrolPoints != null ? patrolPoints.Count : 0;

    public override Vector3 GetPatrolPoint(int patrolIndex) => patrolPoints[patrolIndex];

    public override bool HasPatrolTarget => hasTarget;

    public override void SetPatrolTarget(int patrolIndex)
    {
        currentTarget = patrolPoints[patrolIndex];
        hasTarget = true;
    }

    public override void ClearPatrolTarget()
    {
        hasTarget = false;
    }

    protected override void CreateStates()
    {
        patrolState = new EnemyPatrolState(this, stateMachine, animationController, "Patrol");
        searchState = new EnemySearchState(this, stateMachine, animationController, "Search");
        suspiciousState = new EnemySuspiciousState(this, stateMachine, animationController, "Search");
        pursuitState = new EnemyPursuitState(this, stateMachine, animationController, "Pursuit");
        attackState = new EnemyMageAttackState(this, stateMachine, animationController, "Attack");
    }

    //Scatters `count` NavMesh-sampled patrol points within `radius` of `center` and appends them, so each
    //scout detection piles a fresh hot-zone onto the mage's patrol. Called by MageSpawner.SpawnMage.
    public void AddPatrolArea(Vector3 center, float radius, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 candidate = center + UnityEngine.Random.insideUnitSphere * radius;
            candidate.y = center.y;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, radius, NavMesh.AllAreas))
                patrolPoints.Add(hit.position);
        }
    }
}
