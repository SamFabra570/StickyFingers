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

        

        if (enemy.sight_sensor_.detected_object_ != null)
        {
            //Move towards player, remembering where they are
            enemy.agent_.isStopped = false;
            enemy.lastKnownPlayerPosition = enemy.sight_sensor_.detected_object_.transform.position;
            enemy.agent_.SetDestination(enemy.lastKnownPlayerPosition);
            
            distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.sight_sensor_.detected_object_.transform.position);
            
            if (distanceToTarget <= enemy.attack_distance_)
            {
                stateMachine.ChangeState(new EnemyAttackState(enemy, stateMachine, animationController, "Attack"));
            }
        }
        //Change to search state if player is no longer detected
        else if(enemy.sight_sensor_.detected_object_ == null)
        {
            stateMachine.ChangeState(new EnemySearchState(enemy, stateMachine, animationController, "Search"));
        }
        
        //If close enough to player, switch to attack state
        
    }
}
