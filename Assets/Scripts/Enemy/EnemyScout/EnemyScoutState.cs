using UnityEngine;

public class EnemyScoutState
{
    protected BaseScoutEnemy enemy;
    protected EnemyScoutStateMachine stateMachine;
    protected Animator animationController;
    protected string animationName;

    protected bool isExitingState;
    protected bool isAnimationFinished;
    protected float startTime;
    
    public EnemyScoutState(BaseScoutEnemy _enemy, EnemyScoutStateMachine _stateMachine, Animator _animationController, string _animationName)
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
        SetAnimatorBool(true);
    }

    public virtual void Exit()
    {
        isExitingState = true;
        if (!isAnimationFinished)
            isAnimationFinished = true;
        SetAnimatorBool(false);
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

    //The shared enemy Animator only declares Patrol/Pursuit/Search/Attack. Writing a bool it does not
    //have (e.g. "Stunned") logs a warning on every transition and silently animates nothing, so resolve
    //once per state instance whether the parameter is really there.
    private bool _animParamResolved;
    private bool _animParamExists;

    protected void SetAnimatorBool(bool value)
    {
        if (animationController == null || string.IsNullOrEmpty(animationName))
            return;

        if (!_animParamResolved)
        {
            _animParamResolved = true;
            foreach (AnimatorControllerParameter parameter in animationController.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name == animationName)
                {
                    _animParamExists = true;
                    break;
                }
            }
        }

        if (_animParamExists)
            animationController.SetBool(animationName, value);
    }

    public virtual void AnimationTrigger()
    {
        isAnimationFinished = true;
    }
}

