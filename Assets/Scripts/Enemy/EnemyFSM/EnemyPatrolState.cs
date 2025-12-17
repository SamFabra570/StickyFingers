using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(BaseEnemy _enemy, EnemyStateMachine _stateMachine, Animator _animController, string _animName)
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

        //Switch to pursuit when player is detected
        if (enemy.sight_sensor_.detected_object_ != null)
        {
            stateMachine.ChangeState(new EnemyPursuitState(enemy, stateMachine, animationController, "Pursuit"));
        }
        
        //Move to next waypoint when reached
        else if (enemy.currentTarget != null && !enemy.agent_.pathPending && enemy.agent_.remainingDistance <= enemy.agent_.stoppingDistance + 0.1f)
        {
            enemy.StartCoroutine(enemy.MoveToNextWaypoint());
        }
    }
}
