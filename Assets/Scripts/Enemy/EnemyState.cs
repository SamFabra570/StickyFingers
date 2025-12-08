using UnityEngine;

public class EnemyState
{
    protected BaseEnemy enemy;
    protected EnemyStateMachine stateMachine;
    protected Animator animationController;
    protected string animationName;

    protected bool isExitingState;
    protected bool isAnimationFinished;
    protected float startTime;
    
    public EnemyState(BaseEnemy _enemy, EnemyStateMachine _stateMachine, Animator _animationController, string _animationName)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
        animationController = _animationController;
        animationName = _animationName;
    }

    public virtual void Enter()
    {
        isAnimationFinished = false;
        isExitingState = false;
        startTime = Time.time;
        animationController.SetBool(animationName, true);
    }

    public virtual void Exit()
    {
        isExitingState = true;
        if (!isAnimationFinished)
            isAnimationFinished = true;
        animationController.SetBool(animationName, false);
    }

    public virtual void LogicUpdate()
    {
        TransitionChecks();
    }

    public virtual void PhysicsUpdate()
    {
        
    }

    public virtual void TransitionChecks()
    {
        
    }

    public virtual void AnimationTrigger()
    {
        isAnimationFinished = true;
    }
}
