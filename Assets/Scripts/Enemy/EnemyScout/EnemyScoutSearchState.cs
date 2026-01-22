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
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        //Change to pursuit when detecting player
        if (enemy.sight_sensor_.detected_object_ != null)
        {
            stateMachine.ChangeState(new EnemyScoutPursuitState(enemy, stateMachine, animationController, "Pursuit"));
        }
        
        //Keep moving forward while searching
        enemy.agent_.isStopped = false;
        Vector3 forwardPoint = enemy.transform.position + enemy.transform.forward * enemy.searchDistance;
        enemy.agent_.SetDestination(forwardPoint);

        //Back to patrol when search time ends
        if (Time.time > searchEndTime)
        {
            enemy.currentTarget = null;
            stateMachine.ChangeState(new EnemyScoutPatrolState(enemy, stateMachine, animationController, "Patrol"));
        }
    }
}