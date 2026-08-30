using UnityEngine;

public class EnemyScoutSearchState : EnemyScoutState
{
    private float searchEndTime;

    public EnemyScoutSearchState(BaseScoutEnemy _enemy, EnemyScoutStateMachine _stateMachine, Animator _animController, string _animName)
        : base(_enemy, _stateMachine, _animController, _animName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //Set search time when entering search state
        searchEndTime = Time.time + enemy.searchTime;

        //Head for wherever perception last placed them — which may be an ally's report rather than our
        //own eyes, so an enemy can now search a room it never personally saw anyone enter.
        enemy.agent_.isStopped = false;

        Vector3 destination = enemy.perception != null && enemy.perception.HasLastKnownPosition
            ? enemy.perception.LastKnownPosition
            : enemy.lastKnownPlayerPosition;

        enemy.agent_.SetDestination(destination);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        EnemyPerception perception = enemy.perception;

        if (perception != null && perception.Level == EnemyPerception.Awareness.Alert)
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        //Once the last known position is reached, sweep forward looking for the player
        enemy.agent_.isStopped = false;
        if (!enemy.agent_.pathPending && enemy.agent_.remainingDistance <= 1.0f)
        {
            Vector3 forwardPoint = enemy.transform.position + enemy.transform.forward * enemy.searchDistance;
            enemy.agent_.SetDestination(forwardPoint);
        }

        //Search exhausted. Stand down EXPLICITLY so the post-alert floor is released — otherwise the
        //guard would patrol for ever at a permanently raised awareness and re-trigger off nothing.
        if (Time.time > searchEndTime)
        {
            if (perception != null)
                perception.StandDown();

            enemy.currentTarget = null;
            stateMachine.ChangeState(enemy.patrolState);
        }
    }
}
