using UnityEngine;

public class EnemyScoutPatrolState : EnemyScoutState
{
    public EnemyScoutPatrolState(BaseScoutEnemy _enemy, EnemyScoutStateMachine _stateMachine, Animator _animController, string _animName)
        : base(_enemy, _stateMachine, _animController, _animName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        enemy.fireEffect.SetActive(false);

        //If enemy doesn't have patrol target, find nearest waypoint
        if (enemy.currentTarget == null)
        {
            enemy.FindNearestWaypoint();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        bool seesPlayer = enemy.sight_sensor_.detected_object_ != null;

        //Warmup: must keep the player in sight for detectionWarmup seconds before committing to the attack
        if (enemy.AccumulateSuspicion(seesPlayer))
        {
            enemy.suspicion = 0f;
            stateMachine.ChangeState(new EnemyScoutAttackState(enemy, stateMachine, animationController, "Pursuit"));
            return;
        }

        //Keep patrolling while not yet committed
        if (!seesPlayer && enemy.currentTarget != null && !enemy.agent_.pathPending && enemy.agent_.remainingDistance <= enemy.agent_.stoppingDistance + 0.1f)
        {
            enemy.StartCoroutine(enemy.MoveToNextWaypoint());
        }
    }
}