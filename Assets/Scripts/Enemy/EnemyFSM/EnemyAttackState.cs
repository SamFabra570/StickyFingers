using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private float distanceToTarget;

    private float attackCooldown = 2.5f;
    private float lastAttackTime = -Mathf.Infinity;
    
    
    public EnemyAttackState(BaseEnemy _enemy, EnemyStateMachine _stateMachine, Animator _animController, string _animName)
        : base(_enemy, _stateMachine, _animController, _animName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        Debug.Log("ATTACK STATE");

        enemy.agent_.stoppingDistance = enemy.attack_distance_;
        enemy.agent_.isStopped = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.sight_sensor_.detected_object_ != null)
        {
            distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.sight_sensor_.detected_object_.transform.position);

            if (distanceToTarget <= enemy.attack_distance_)
            {
                enemy.agent_.isStopped = true;

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    PlayerController player = enemy.sight_sensor_.detected_object_.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        player.FreezeMovement();
                        GameManager.Instance.inventorySystem.Remove(player.inventoryItem);
                        lastAttackTime = Time.time;
                        
                    }
                }
            }
            else
            {
                enemy.agent_.isStopped = false;
            }
            
            //If player gets too far, switch back to pursuit
            if (distanceToTarget > enemy.attack_distance_ * enemy.stop_attack_distance_multiplier)
            {
                stateMachine.ChangeState(new EnemyPursuitState(enemy, stateMachine, animationController, "Pursuit"));
            }
        }
        //If player no longer detected, go back to patrol
        else if (enemy.sight_sensor_.detected_object_ == null)
        {
            stateMachine.ChangeState(new EnemyPatrolState(enemy, stateMachine, animationController, "Patrol"));
        }
        
        
    }
}
