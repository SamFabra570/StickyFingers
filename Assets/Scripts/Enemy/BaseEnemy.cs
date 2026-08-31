using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : EnemyBrain
{
    //Designer-placed patrol route. DemonHelper reads this list, so it stays a List<Transform> here
    //rather than being flattened into world positions in the base class.
    public List<Transform> waypoints;
    public Transform currentTarget;

    public GameObject stunEffect;
    public EnemyStunnedState stunnedState;

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

    //The guard is the only enemy that retreats after landing a hit, to give the frozen player a chance.
    public override bool UsesPostAttackBackoff => true;

    protected override void Start()
    {
        if (stunEffect != null)
            stunEffect.SetActive(false);

        base.Start();
    }

    protected override void CreateStates()
    {
        patrolState = new EnemyPatrolState(this, stateMachine, animationController, "Patrol");
        searchState = new EnemySearchState(this, stateMachine, animationController, "Search");
        //No "Suspicious" bool exists on the shared Animator, so it borrows the Search look — which is
        //exactly what the state is doing anyway: standing there, looking around.
        suspiciousState = new EnemySuspiciousState(this, stateMachine, animationController, "Search");
        pursuitState = new EnemyPursuitState(this, stateMachine, animationController, "Pursuit");
        attackState = new EnemyAttackState(this, stateMachine, animationController, "Attack");
        stunnedState = new EnemyStunnedState(this, stateMachine, animationController, "Stunned");
    }

    public void Stun(float duration)
    {
        stunnedState.SetStunDuration(duration);
        stateMachine.ChangeState(stunnedState);
    }
}
