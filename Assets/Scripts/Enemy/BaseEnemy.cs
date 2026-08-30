using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    public EnemyStateMachine stateMachine;
    public EnemyState patrolState;
    public EnemyState searchState;
    public EnemyState suspiciousState;
    public EnemyState pursuitState;
    public EnemyState attackState;
    public EnemyStunnedState stunnedState;
    
    public Animator animationController;
    
    public Sight sight_sensor_;
    [HideInInspector] public EnemyPerception perception;
    public NavMeshAgent agent_;
    
    public GameObject fireEffect;
    public GameObject stunEffect;
    
    //Patrolling
    public List<Transform> waypoints;
    public Transform currentTarget;
    private int index = 0;
    public bool isMovingToWaypoint = false;
    public float patrolWaitTime = 2.0f;
    [Tooltip("Random +/- variation applied to patrolWaitTime so a route cannot be timed with a stopwatch.")]
    public float patrolWaitJitter = 1.5f;
    [Tooltip("How far the guard sweeps its vision cone left/right while paused at a waypoint.")]
    public float patrolScanAngle = 65.0f;
    [Tooltip("Chance at each waypoint that the guard turns around and walks the route backwards instead. A loop that always runs the same way is a loop you memorise once.")]
    public float patrolReverseChance = 0.2f;
    private int patrolDirection = 1;

    //Searching for player
    private Vector3 searchDir;
    //private bool isSearching = false;
    public float searchDistance = 5.0f;
    public float searchTime = 10.0f;
    public Vector3 lastKnownPlayerPosition;
    private Vector3 forwardPoint;
    
    //Attacking
    public float attack_distance_ = 2.0f;
    public float stop_attack_distance_multiplier = 1.2f;

    //After a successful hit, the guard must wait this long before attacking again, and Pursuit will back off during the window so the (freshly unfrozen) player has a chance to escape.
    public float attackCooldown = 5f;
    public float postAttackBackoffDistance = 3f;
    public float attackAnimHold = 0.6f;
    [HideInInspector] public float lastAttackTime = -Mathf.Infinity;

    //SUPERSEDED by EnemyPerception (decayDelay + alertExit + alertedFloor), which now decides when
    //an enemy gives up. Kept only so older prefabs deserialise cleanly; tuning it changes nothing.
    public float loseSightGracePeriod = 3.5f;
    [HideInInspector] public float lastSeenTime = -Mathf.Infinity;
    
    [SerializeField] public bool isBeingSeen;

    //Visibility-based speed — fast while the player is watching, slow while not. Pursuit overrides both with pursuitSpeed so the chase feels urgent.
    public float hiddenSpeed = 1.5f;
    public float visibleSpeed = 3.0f;
    public float pursuitSpeed = 6.0f;
    private DitherVisibility ditherVisibility_;

    private void Awake()
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireEffect.SetActive(false);
        stunEffect.SetActive(false);
        
        if (waypoints.Count > 0 && waypoints[0] != null)
        {
            //Set first target
            currentTarget = waypoints[index];
            
            //Start moving towards first target
            agent_.SetDestination(currentTarget.position);
        }
        
        //Initialize state machine
        stateMachine = new EnemyStateMachine();
        
        //Create state instances
        patrolState = new EnemyPatrolState(this, stateMachine, animationController, "Patrol");
        searchState = new EnemySearchState(this, stateMachine, animationController, "Search");
        //No "Suspicious" bool exists on the shared Animator, so it borrows the Search look —
        //which is exactly what the state is doing anyway: standing there, looking around.
        suspiciousState = new EnemySuspiciousState(this, stateMachine, animationController, "Search");
        pursuitState = new EnemyPursuitState(this, stateMachine, animationController, "Pursuit");
        attackState = new EnemyAttackState(this, stateMachine, animationController, "Attack");
        stunnedState = new EnemyStunnedState(this, stateMachine, animationController, "Stunned");

        //Start patrol state
        stateMachine.InitializeStateMachine(patrolState);
    }

    private void Update()
    {
        UpdateSpeed();

        if (stateMachine._CurrentState != null)
            stateMachine._CurrentState.LogicUpdate();
    }

    //Move fast while the player can see this enemy, slow while they can't. Pursuit overrides this with pursuitSpeed.
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
    
    void FixedUpdate()
    {
        if (stateMachine._CurrentState != null)
            stateMachine._CurrentState.PhysicsUpdate();
    }

    //Sweeps the vision cone to both sides while the guard is paused at a waypoint. Aborts the moment we
    //leave patrol so a stale sweep never fights Pursuit for control of the transform.
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

        if (waypoints == null || waypoints.Count == 0)
        {
            isMovingToWaypoint = false;
            yield break;
        }
        
        //Pause movement at each waypoint
        agent_.isStopped = true;
        //A guard that pauses for exactly patrolWaitTime and stares at one wall is a guard you can time
        //with a stopwatch. Randomise the pause and sweep the cone across it instead.
        object patrolling = stateMachine._CurrentState;
        yield return ScanWhileWaiting(patrolling);

        //This coroutine outlives the state that started it: if we were pulled into Pursuit/Search while
        //paused, do NOT carry on and overwrite the agent's destination with a patrol waypoint.
        if (!ReferenceEquals(stateMachine._CurrentState, patrolling))
        {
            isMovingToWaypoint = false;
            yield break;
        }

        //Clamp waypoint index
        index = Mathf.Clamp(index, 0, waypoints.Count - 1);
        
        //Occasionally turn back the way we came, and wrap in whichever direction we are travelling.
        if (UnityEngine.Random.value < patrolReverseChance)
            patrolDirection = -patrolDirection;

        index += patrolDirection;

        if (index < 0)
            index = waypoints.Count - 1;

        //Loop if reached the last waypoint
        if (index >= waypoints.Count)
            index = 0;
        
        currentTarget = waypoints[index];
        
        //Move to next waypoint
        agent_.isStopped = false;
        agent_.SetDestination(currentTarget.position);
        
        isMovingToWaypoint = false;
    }
    
    public void FindNearestWaypoint()
    {
        Transform nearestWaypoint = null;
        float nearestDistance = float.MaxValue;

        //Check distance of each waypoint, find nearest waypoint
        for (int i = 0; i < waypoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].position);
            
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestWaypoint = waypoints[i];
            }
        }
        
        //Go to nearest waypoint once it has been set
        if (nearestWaypoint != null)
        {
            agent_.isStopped = false;
            agent_.SetDestination(nearestWaypoint.position);
            currentTarget = nearestWaypoint;
            index = waypoints.IndexOf(nearestWaypoint);
        }
    }

    public void Stun(float duration)
    {
        stunnedState.SetStunDuration(duration);
        
        stateMachine.ChangeState(stunnedState);
    }
}
