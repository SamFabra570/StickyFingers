using UnityEngine;

public class EnemyMageAttackState : EnemyState
{
    private float distanceToTarget;

    private float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;
    
    [SerializeField] private float mapRange = 50f;
    
    //private bool isTeleporting = false;
    private Transform playerTransform;
    
    public EnemyMageAttackState(EnemyBrain _enemy, EnemyStateMachine _stateMachine, Animator _animController, string _animName)
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
                    // detected_object_ may be a child collider (wings/ability hitboxes share the Player layer), so walk up to the root.
                    PlayerController player = enemy.sight_sensor_.detected_object_.GetComponentInParent<PlayerController>();
                    
                    if (player != null)
                    {
                        var passive = GameManager.Instance.PlayerPassives;
                        
                        if (passive.Has(PassiveAbilities.SecondChance) && !player.hasUsedSecondChance)
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

                            Debug.Log("Second chance used");
                            player.hasUsedSecondChance = true;
                        }
                        else
                            GameManager.Instance.EndGame(false, "Mage");
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
                stateMachine.ChangeState(enemy.pursuitState);
            }
        }
        //Losing sight mid-fight used to send the enemy straight back to its patrol route — the single
        //most generous piece of amnesia in the whole AI: you were being hit a frame ago and now nobody
        //is looking for you. Perception decides instead. Still certain? keep chasing. Otherwise search.
        //Patrol is only ever reached once awareness has genuinely decayed, which takes real time.
        else if (enemy.perception == null || !enemy.perception.HasVisual)
        {
            if (enemy.perception != null && enemy.perception.Level == EnemyPerception.Awareness.Alert)
                stateMachine.ChangeState(enemy.pursuitState);
            else
                stateMachine.ChangeState(enemy.searchState);
        }
        
        
    }
}
