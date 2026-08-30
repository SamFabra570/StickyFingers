using UnityEngine;

public class EnemyScoutPursuitState : EnemyScoutState
{
    private float distanceToTarget;

    public EnemyScoutPursuitState(BaseScoutEnemy _enemy, EnemyScoutStateMachine _stateMachine, Animator _animController, string _animName)
        : base(_enemy, _stateMachine, _animController, _animName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fireEffect.SetActive(true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        EnemyPerception perception = enemy.perception;

        if (perception == null)
        {
            //Should never happen — the enemy builds one in Awake. Fail towards searching, not freezing.
            stateMachine.ChangeState(enemy.searchState);
            return;
        }

        //Eyes-on. The raw sensor is the right source for WHERE, but only once perception has already
        //decided we are committed: deciding and measuring are different jobs and must not share a source.
        if (perception.HasVisual)
        {
            Vector3 playerPos = perception.VisualTarget.transform.position;

            enemy.agent_.isStopped = false;
            enemy.lastKnownPlayerPosition = playerPos;
            enemy.lastSeenTime = Time.time;
            enemy.agent_.SetDestination(playerPos);

            distanceToTarget = Vector3.Distance(enemy.transform.position, playerPos);

            if (distanceToTarget <= enemy.attack_distance_)
            {
                stateMachine.ChangeState(enemy.attackState);
            }

            return;
        }

        //Lost visual. Giving up is perception's call now, not a stopwatch: awareness holds flat through
        //decayDelay, then falls, and cannot drop below the post-alert floor for a while. That is what
        //stops "duck behind a wall for two seconds" from being a complete reset.
        if (perception.Level == EnemyPerception.Awareness.Alert)
        {
            if (perception.HasLastKnownPosition)
            {
                enemy.lastKnownPlayerPosition = perception.LastKnownPosition;
                enemy.agent_.isStopped = false;
                enemy.agent_.SetDestination(perception.LastKnownPosition);
            }

            return;
        }

        stateMachine.ChangeState(enemy.searchState);
    }
}
