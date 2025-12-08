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

        //Start moving if enemy has waypoint target
        if (enemy.waypoints.Count > 0 && enemy.currentTarget != null)
        {
            enemy.StartCoroutine(enemy.MoveToNextWaypoint());
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
        
        else if (enemy.currentTarget != null && Vector3.Distance(enemy.transform.position, enemy.currentTarget.position) <= 2f)
        {
            //Move to next waypoint when reached
            enemy.StartCoroutine(enemy.MoveToNextWaypoint());
        }
    }
}
