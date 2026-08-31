using UnityEngine;

public class EnemyStunnedState : EnemyState
{
    //Everything else works off EnemyBrain, but the stun VFX is the guard's own, so keep a typed handle.
    private readonly BaseEnemy guard;

    private float stunDuration;

    public EnemyStunnedState(BaseEnemy _enemy, EnemyStateMachine _stateMachine, Animator _animator, string _animationName)
        : base(_enemy, _stateMachine, _animator, _animationName)
    {
        guard = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.fireEffect != null)
            enemy.fireEffect.SetActive(false);

        if (guard.stunEffect != null)
            guard.stunEffect.SetActive(true);

        enemy.agent_.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();

        if (guard.stunEffect != null)
            guard.stunEffect.SetActive(false);

        enemy.agent_.isStopped = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + stunDuration)
            stateMachine.ChangeState(enemy.patrolState);
    }

    public void SetStunDuration(float duration)
    {
        stunDuration = duration;
    }
}
