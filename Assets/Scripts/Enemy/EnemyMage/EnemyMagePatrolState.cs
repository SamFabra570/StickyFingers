using UnityEngine;

public class EnemyMagePatrolState : EnemyMageState
{
    public EnemyMagePatrolState(BaseMageEnemy _enemy, EnemyMageStateMachine _stateMachine, Animator _animController, string _animName)
        : base(_enemy, _stateMachine, _animController, _animName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fireEffect.SetActive(false);

        //If enemy doesn't have patrol target, find nearest waypoint
        if (!enemy.hasTarget)
        {
            enemy.FindNearestWaypoint();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        EnemyPerception perception = enemy.perception;

        //Patrol asks perception, never the raw ray — and it can no longer jump straight into a chase.
        //The most a glimpse can do from here is make the guard curious enough to walk over and look.
        //Alert only lands here when something ELSE already made us certain (an ally shouting, a noise),
        //and in that case committing immediately is the correct response, not a bug.
        if (perception != null)
        {
            if (perception.Level == EnemyPerception.Awareness.Alert)
            {
                stateMachine.ChangeState(enemy.pursuitState);
                return;
            }

            if (perception.Level == EnemyPerception.Awareness.Suspicious)
            {
                stateMachine.ChangeState(enemy.suspiciousState);
                return;
            }
        }

        //Move to next waypoint when reached
        if (enemy.hasTarget && !enemy.agent_.pathPending && enemy.agent_.remainingDistance <= enemy.agent_.stoppingDistance + 0.1f)
        {
            enemy.StartCoroutine(enemy.MoveToNextWaypoint());
        }
    }
}
