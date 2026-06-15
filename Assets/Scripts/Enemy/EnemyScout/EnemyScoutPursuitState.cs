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

        if (enemy.sight_sensor_.detected_object_ != null)
        {
            enemy.agent_.isStopped = false;
            enemy.lastKnownPlayerPosition = enemy.sight_sensor_.detected_object_.transform.position;
            enemy.lastSeenTime = Time.time;
            enemy.agent_.SetDestination(enemy.lastKnownPlayerPosition);

            distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.sight_sensor_.detected_object_.transform.position);

            if (distanceToTarget <= enemy.attack_distance_)
            {
                stateMachine.ChangeState(new EnemyScoutAttackState(enemy, stateMachine, animationController, "Attack"));
            }
            return;
        }

        //Lost sight: keep heading to the last known position during the grace period, only fall back to Search after it expires.
        if (Time.time - enemy.lastSeenTime < enemy.loseSightGracePeriod)
        {
            enemy.agent_.isStopped = false;
            enemy.agent_.SetDestination(enemy.lastKnownPlayerPosition);
            return;
        }

        stateMachine.ChangeState(new EnemyScoutSearchState(enemy, stateMachine, animationController, "Search"));
    }
}
