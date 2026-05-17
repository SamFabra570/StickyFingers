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

        //Head straight for where the player was last seen
        enemy.agent_.isStopped = false;
        enemy.agent_.SetDestination(enemy.lastKnownPlayerPosition);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        //Scout alerts the Mage the instant it spots the player — straight to attack
        if (enemy.sight_sensor_.detected_object_ != null)
        {
            stateMachine.ChangeState(new EnemyScoutAttackState(enemy, stateMachine, animationController, "Pursuit"));
            return;
        }

        //Once the last known position is reached, sweep forward looking for the player
        enemy.agent_.isStopped = false;
        if (!enemy.agent_.pathPending && enemy.agent_.remainingDistance <= 1.0f)
        {
            Vector3 forwardPoint = enemy.transform.position + enemy.transform.forward * enemy.searchDistance;
            enemy.agent_.SetDestination(forwardPoint);
        }

        //Back to patrol when search time ends
        if (Time.time > searchEndTime)
        {
            enemy.currentTarget = null;
            stateMachine.ChangeState(new EnemyScoutPatrolState(enemy, stateMachine, animationController, "Patrol"));
        }
    }
}