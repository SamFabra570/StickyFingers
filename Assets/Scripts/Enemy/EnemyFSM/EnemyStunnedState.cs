using UnityEngine;
using UnityEngine.AI;

public class EnemyStunnedState : EnemyState
{
    private float stunDuration;

    public EnemyStunnedState(
        BaseEnemy enemy,
        EnemyStateMachine stateMachine,
        Animator animator,
        string animationName
    ) : base(enemy, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fireEffect.SetActive(false);
        enemy.stunEffect.SetActive(true);
        enemy.agent_.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.stunEffect.SetActive(false);
        enemy.agent_.isStopped = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(Time.time >= startTime + stunDuration)
        {
            stateMachine.ChangeState(enemy.patrolState);
        }
    }
    
    public void SetStunDuration(float duration)
    {
        stunDuration = duration;
    }
}