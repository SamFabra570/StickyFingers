using UnityEngine;

public class EnemyMageSearchState : EnemyMageState
{
    private float searchEndTime;
    
    public EnemyMageSearchState(BaseMageEnemy _enemy, EnemyMageStateMachine _stateMachine, Animator _animController, string _animName)
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
        
        //Change to pursuit when detecting player
        if (enemy.sight_sensor_.detected_object_ != null)
        {
            stateMachine.ChangeState(new EnemyMagePursuitState(enemy, stateMachine, animationController, "Pursuit"));
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
            stateMachine.ChangeState(new EnemyMagePatrolState(enemy, stateMachine, animationController, "Patrol"));
        }
    }
}
