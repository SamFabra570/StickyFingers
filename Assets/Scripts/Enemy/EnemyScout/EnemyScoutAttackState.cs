using UnityEngine;
using System;
public class EnemyScoutAttackState : EnemyState
{
    private float distanceToTarget;

    private float attackCooldown = 2.5f;
    private float lastAttackTime = -Mathf.Infinity;
    
    
    public EnemyScoutAttackState(EnemyBrain _enemy, EnemyStateMachine _stateMachine, Animator _animController, string _animName)
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
            enemy.agent_.isStopped = true;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // detected_object_ may be a child collider (wings/ability hitboxes share the Player layer), so walk up to the root.
                PlayerController player = enemy.sight_sensor_.detected_object_.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    //player.FreezeMovement(2);
                    Debug.Log("Intruso encontrado, ALERTAR AL MAGOOO !!!");
                    //Pass WHERE the player was spotted so the mage patrols that hot-zone (additive per scout).
                    Vector3 detectionPoint = enemy.sight_sensor_.detected_object_.transform.position;
                    MageSpawner.Instance.SpawnMage(detectionPoint);
                    lastAttackTime = Time.time;
                    enemy.gameObject.SetActive(false);
                }
            }
            
            else
            {
                enemy.agent_.isStopped = false;
            }
            
            //If player gets too far, switch back to pursuit
            /*if (distanceToTarget > enemy.attack_distance_ * enemy.stop_attack_distance_multiplier)
            {
                stateMachine.ChangeState(enemy.pursuitState);
            }*/
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

