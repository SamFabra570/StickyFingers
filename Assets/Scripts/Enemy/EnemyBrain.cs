using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//Everything three near-identical enemy classes used to each own a copy of. The families genuinely differ
//in exactly two places, and both are abstracted here rather than duplicated:
//
//  1. WHERE they patrol. The guard and the scout walk a designer-placed List<Transform>; the mage walks a
//     List<Vector3> generated at runtime by MageSpawner. Both are reachable through PatrolPointCount /
//     GetPatrolPoint, so the patrol code does not care which it is.
//  2. WHAT certainty commits them to. Guard and mage chase. The scout does not fight at all — it runs off
//     to fetch a mage — so it overrides AlertState.
//
//Everything else was copy-paste, and copy-paste is why this AI cost 3x to fix: every change had to be
//made three times, and the third one always got forgotten.
public abstract class EnemyBrain : MonoBehaviour
{
    public EnemyStateMachine stateMachine;
    public EnemyState patrolState;
    public EnemyState searchState;
    public EnemyState suspiciousState;
    public EnemyState pursuitState;
    public EnemyState attackState;

    public Animator animationController;

    public Sight sight_sensor_;
    [HideInInspector] public EnemyPerception perception;
    public NavMeshAgent agent_;

    public GameObject fireEffect;

    //Patrolling
    public float patrolWaitTime = 2.0f;
    [Tooltip("Random +/- variation applied to patrolWaitTime so a route cannot be timed with a stopwatch.")]
    public float patrolWaitJitter = 1.5f;
    [Tooltip("How far the enemy sweeps its vision cone left/right while paused at a waypoint.")]
    public float patrolScanAngle = 65.0f;
    [Tooltip("Chance at each waypoint of turning around and walking the route backwards instead. A loop that always runs the same way is a loop you memorise once.")]
    public float patrolReverseChance = 0.2f;

    protected int index = 0;
    protected bool isMovingToWaypoint = false;
    private int patrolDirection = 1;

    //Searching for player
    public float searchDistance = 5.0f;
    public float searchTime = 10.0f;
    public Vector3 lastKnownPlayerPosition;

    //Attacking
    public float attack_distance_ = 2.0f;
    public float stop_attack_distance_multiplier = 1.2f;

    //After a successful hit, wait this long before attacking again. Only enemies that opt into
    //UsesPostAttackBackoff also retreat during the window.
    public float attackCooldown = 5.0f;
    public float postAttackBackoffDistance = 3.0f;
    public float attackAnimHold = 0.6f;
    [HideInInspector] public float lastAttackTime = -Mathf.Infinity;

    //SUPERSEDED by EnemyPerception (decayDelay + alertExit + alertedFloor), which now decides when an
    //enemy gives up. Kept only so older prefabs deserialise cleanly; tuning it changes nothing.
    public float loseSightGracePeriod = 3.5f;
    [HideInInspector] public float lastSeenTime = -Mathf.Infinity;

    [SerializeField] public bool isBeingSeen;

    //Visibility-based speed — fast while the player is watching, slow while not. Pursuit overrides both.
    public float hiddenSpeed = 1.5f;
    public float visibleSpeed = 3.0f;
    public float pursuitSpeed = 6.0f;
    private DitherVisibility ditherVisibility_;

    // ---- the two things the families genuinely disagree about -------------------------------------

    public abstract int PatrolPointCount { get; }
    public abstract Vector3 GetPatrolPoint(int patrolIndex);
    public abstract bool HasPatrolTarget { get; }
    public abstract void SetPatrolTarget(int patrolIndex);
    public abstract void ClearPatrolTarget();

    //What certainty commits this enemy to. Overridden by the scout, which fetches a mage instead of fighting.
    public virtual EnemyState AlertState => pursuitState;

    //Only the guard retreats after landing a hit, to give the frozen player room to escape.
    public virtual bool UsesPostAttackBackoff => false;

    protected abstract void CreateStates();

    // ----------------------------------------------------------------------------------------------

    protected virtual void Awake()
    {
        agent_ = GetComponent<NavMeshAgent>();
        ditherVisibility_ = GetComponentInChildren<DitherVisibility>();

        //Perception is wired up in code rather than authored on the prefab, so nothing has to be
        //re-serialised to get it. If someone later adds one by hand to tune the numbers, that one wins.
        perception = GetComponent<EnemyPerception>();
        if (perception == null)
            perception = gameObject.AddComponent<EnemyPerception>();

        perception.Initialise(sight_sensor_);
    }

    protected virtual void Start()
    {
        if (fireEffect != null)
            fireEffect.SetActive(false);

        if (PatrolPointCount > 0)
        {
            index = 0;
            SetPatrolTarget(index);
            agent_.SetDestination(GetPatrolPoint(index));
        }

        stateMachine = new EnemyStateMachine();
        CreateStates();
        stateMachine.InitializeStateMachine(patrolState);
    }

    protected virtual void Update()
    {
        UpdateSpeed();

        if (stateMachine._CurrentState != null)
            stateMachine._CurrentState.LogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        if (stateMachine != null && stateMachine._CurrentState != null)
            stateMachine._CurrentState.PhysicsUpdate();
    }

    //Move fast while the player can see this enemy, slow while they can't. Pursuit overrides with pursuitSpeed.
    private void UpdateSpeed()
    {
        if (stateMachine != null && stateMachine._CurrentState is EnemyPursuitState)
        {
            agent_.speed = pursuitSpeed;
            return;
        }

        if (ditherVisibility_ == null)
            return;

        agent_.speed = ditherVisibility_.IsVisible ? visibleSpeed : hiddenSpeed;
    }

    //Sweeps the vision cone to both sides while paused at a waypoint. Aborts the moment we leave patrol
    //so a stale sweep never fights Pursuit for control of the transform.
    private IEnumerator ScanWhileWaiting(object patrolling)
    {
        float wait = Mathf.Max(0.25f, patrolWaitTime + UnityEngine.Random.Range(-patrolWaitJitter, patrolWaitJitter));

        //The agent owns rotation while it steers; borrow it for the sweep and always hand it back.
        bool hadUpdateRotation = agent_.updateRotation;
        agent_.updateRotation = false;

        yield return TurnBy(UnityEngine.Random.Range(-patrolScanAngle, patrolScanAngle), wait * 0.5f, patrolling);
        yield return TurnBy(UnityEngine.Random.Range(-patrolScanAngle, patrolScanAngle), wait * 0.5f, patrolling);

        agent_.updateRotation = hadUpdateRotation;
    }

    private IEnumerator TurnBy(float degrees, float duration, object patrolling)
    {
        Quaternion from = transform.rotation;
        Quaternion to = from * Quaternion.Euler(0.0f, degrees, 0.0f);

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            if (!ReferenceEquals(stateMachine._CurrentState, patrolling))
                yield break;

            transform.rotation = Quaternion.Slerp(from, to, Mathf.Clamp01(t / duration));
            yield return null;
        }

        transform.rotation = to;
    }

    public IEnumerator MoveToNextWaypoint()
    {
        //if moving, don't call again
        if (isMovingToWaypoint)
            yield break;

        isMovingToWaypoint = true;

        if (PatrolPointCount == 0)
        {
            ClearPatrolTarget();
            isMovingToWaypoint = false;
            yield break;
        }

        //Pause movement at each waypoint, randomised and with a cone sweep so the route cannot be timed.
        agent_.isStopped = true;

        object patrolling = stateMachine._CurrentState;
        yield return ScanWhileWaiting(patrolling);

        //This coroutine outlives the state that started it: if we were pulled into Pursuit/Search while
        //paused, do NOT carry on and overwrite the agent's destination with a patrol waypoint.
        if (!ReferenceEquals(stateMachine._CurrentState, patrolling))
        {
            isMovingToWaypoint = false;
            yield break;
        }

        index = Mathf.Clamp(index, 0, PatrolPointCount - 1);

        //Occasionally turn back the way we came, and wrap in whichever direction we are travelling.
        if (UnityEngine.Random.value < patrolReverseChance)
            patrolDirection = -patrolDirection;

        index += patrolDirection;

        if (index < 0)
            index = PatrolPointCount - 1;

        if (index >= PatrolPointCount)
            index = 0;

        SetPatrolTarget(index);

        agent_.isStopped = false;
        agent_.SetDestination(GetPatrolPoint(index));

        isMovingToWaypoint = false;
    }

    public void FindNearestWaypoint()
    {
        if (PatrolPointCount == 0)
        {
            ClearPatrolTarget();
            return;
        }

        int nearest = 0;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < PatrolPointCount; i++)
        {
            float distance = Vector3.Distance(transform.position, GetPatrolPoint(i));

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = i;
            }
        }

        index = nearest;
        SetPatrolTarget(index);

        agent_.isStopped = false;
        agent_.SetDestination(GetPatrolPoint(index));
    }
}
