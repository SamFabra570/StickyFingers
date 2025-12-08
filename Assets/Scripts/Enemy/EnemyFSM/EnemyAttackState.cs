using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(BaseEnemy _enemy, EnemyStateMachine _stateMachine, Animator _animController, string _animName)
        : base(_enemy, _stateMachine, _animController, _animName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent_.isStopped = true;
        
        //Add attack effect here if any
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //If player no longer detected, go back to patrol
        if (enemy.sight_sensor_.detected_object_ == null)
        {
            stateMachine.ChangeState(new EnemyPatrolState(enemy, stateMachine, animationController, "Patrol"));
        }
        
        float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.sight_sensor_.detected_object_.transform.position);
        
        //If player gets too far, switch back to pursuit
        if (distanceToTarget > enemy.attack_distance_ * enemy.stop_attack_distance_multiplier)
        {
            stateMachine.ChangeState(new EnemyPursuitState(enemy, stateMachine, animationController, "Pursuit"));
        }
    }
}
