using UnityEngine;

//One state base for every enemy. It talks to EnemyBrain, not to a specific enemy type, which is what
//lets Patrol/Suspicious/Search/Pursuit exist once instead of three times. States that genuinely need a
//particular enemy (the guard's inventory theft, the scout's summon) keep their own typed reference.
public abstract class EnemyState
{
    protected EnemyBrain enemy;
    protected EnemyStateMachine stateMachine;
    protected Animator animationController;
    protected string animationName;

    protected bool isExitingState;
    protected bool isAnimationFinished;
    protected float startTime;

    private bool _animParamResolved;
    private bool _animParamExists;

    public EnemyState(EnemyBrain _enemy, EnemyStateMachine _stateMachine, Animator _animationController, string _animationName)
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

    public virtual void AnimationTrigger()
    {
        isAnimationFinished = true;
    }

    //The shared enemy Animator only declares Patrol/Pursuit/Search/Attack. Writing a bool it does not
    //have (e.g. "Stunned") logs a warning on every transition and silently animates nothing, so resolve
    //once per state instance whether the parameter is really there.
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
}
