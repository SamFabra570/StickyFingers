using UnityEngine;

public class EnemyPursuitState : EnemyState
{
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
        
        if(enemy.sight_sensor_.detected_object_ == null)
        {
            stateMachine.ChangeState(new EnemySearchState(enemy, stateMachine, animationController, "Search"));
        }
        
        //Move towards player
        enemy.agent_.isStopped = false;
        enemy.agent_.SetDestination(enemy.sight_sensor_.detected_object_.transform.position);

        float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.sight_sensor_.detected_object_.transform.position);
        
        //If close enough to player, switch to attack state
        if (distanceToTarget <= enemy.attack_distance_)
        {
            stateMachine.ChangeState(new EnemyAttackState(enemy, stateMachine, animationController, "Attack"));
        }
    }
}
