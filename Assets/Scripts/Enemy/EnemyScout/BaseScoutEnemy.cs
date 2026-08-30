using System.Collections.Generic;
using UnityEngine;

public class BaseScoutEnemy : EnemyBrain
{
    //Designer-placed patrol route.
    public List<Transform> waypoints;
    public Transform currentTarget;

    //SUPERSEDED by EnemyPerception, which does this for every enemy with hysteresis and cover weighting
    //instead of only for the scout. Nothing reads these any more; kept so existing prefabs deserialise
    //cleanly. Delete both once you are happy with perception.
    [Tooltip("SUPERSEDED by EnemyPerception — tuning this does nothing.")]
    public float detectionWarmup = 2.5f;
    public float suspicion;

    public override int PatrolPointCount => waypoints != null ? waypoints.Count : 0;

    public override Vector3 GetPatrolPoint(int patrolIndex)
    {
        Transform point = waypoints[patrolIndex];
        return point != null ? point.position : transform.position;
    }

    public override bool HasPatrolTarget => currentTarget != null;

    public override void SetPatrolTarget(int patrolIndex)
    {
        currentTarget = waypoints[patrolIndex];
    }

    public override void ClearPatrolTarget()
    {
        currentTarget = null;
    }

    //The scout does not fight. Certainty sends it to its "attack" state, which is really spot-the-intruder
    //and go fetch a mage — so Alert must commit here rather than to a pursuit it would never win.
    public override EnemyState AlertState => attackState;

    protected override void CreateStates()
    {
        patrolState = new EnemyPatrolState(this, stateMachine, animationController, "Patrol");
        searchState = new EnemySearchState(this, stateMachine, animationController, "Search");
        suspiciousState = new EnemySuspiciousState(this, stateMachine, animationController, "Search");
        pursuitState = new EnemyPursuitState(this, stateMachine, animationController, "Pursuit");
        //The scout has no attack animation because it never attacks — it runs at you to identify you,
        //so the pursuit look is the correct one here.
        attackState = new EnemyScoutAttackState(this, stateMachine, animationController, "Pursuit");
    }
}
