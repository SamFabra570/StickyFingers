using UnityEngine;

public class EnemyPursuitState : EnemyState
{
    private float distanceToTarget;

    public EnemyPursuitState(BaseEnemy _enemy, EnemyStateMachine _stateMachine, Animator _animController, string _animName)
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

        bool sighted = enemy.sight_sensor_.detected_object_ != null;

        if (sighted)
        {
            Vector3 playerPos = enemy.sight_sensor_.detected_object_.transform.position;
            enemy.lastKnownPlayerPosition = playerPos;
            enemy.lastSeenTime = Time.time;
            distanceToTarget = Vector3.Distance(enemy.transform.position, playerPos);

            bool cooldownActive = Time.time < enemy.lastAttackTime + enemy.attackCooldown;

            //After a hit, give the (frozen) player room to escape: walk AWAY from them until cooldown clears.
            if (cooldownActive && distanceToTarget <= enemy.attack_distance_)
            {
                Vector3 away = (enemy.transform.position - playerPos);
                if (away.sqrMagnitude < 0.0001f)
                    away = -enemy.transform.forward;
                else
                    away.Normalize();

                enemy.agent_.isStopped = false;
                enemy.agent_.SetDestination(enemy.transform.position + away * enemy.postAttackBackoffDistance);
                return;
            }

            enemy.agent_.isStopped = false;
            enemy.agent_.SetDestination(playerPos);

            if (!cooldownActive && distanceToTarget <= enemy.attack_distance_)
            {
                stateMachine.ChangeState(new EnemyAttackState(enemy, stateMachine, animationController, "Attack"));
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

        stateMachine.ChangeState(new EnemySearchState(enemy, stateMachine, animationController, "Search"));
    }
}
