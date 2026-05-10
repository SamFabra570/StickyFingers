using UnityEngine;

public class EnemyMageAttackState : EnemyMageState
{
    private float distanceToTarget;

    private float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;

    private bool hasUsedSecondChance;
    
    [SerializeField] private float mapRange = 50f;
    
    //private bool isTeleporting = false;
    private Transform playerTransform;
    
    public EnemyMageAttackState(BaseMageEnemy _enemy, EnemyMageStateMachine _stateMachine, Animator _animController, string _animName)
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
                        var passive = GameManager.Instance.PlayerPassives;
                        
                        if (passive.Has(PassiveAbilities.SecondChance) && !hasUsedSecondChance)
                        {
                            playerTransform = player.transform;
                            
                            if (RandomNavMeshPoint.TryGetRandomPoint(mapRange, out Vector3 destination))
                            {
                                //isTeleporting = true;
            
                                CharacterController cc = playerTransform.GetComponent<CharacterController>();
            
                                destination.y += cc.height / 2f + cc.skinWidth;
                                cc.enabled = false;
                                playerTransform.position = destination;
                                cc.enabled = true;
                            }
                            else
                            {
                                Debug.LogWarning("SecondChance: Could not find a valid NavMesh point to teleport to.");
                            }

                            hasUsedSecondChance = true;
                        }
                        else 
                            GameManager.Instance.EndGame();
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
                stateMachine.ChangeState(new EnemyMagePursuitState(enemy, stateMachine, animationController, "Pursuit"));
            }
        }
        //If player no longer detected, go back to patrol
        else if (enemy.sight_sensor_.detected_object_ == null)
        {
            stateMachine.ChangeState(new EnemyMagePatrolState(enemy, stateMachine, animationController, "Patrol"));
        }
        
        
    }
}
