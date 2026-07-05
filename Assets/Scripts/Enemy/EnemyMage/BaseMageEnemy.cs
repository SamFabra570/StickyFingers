using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BaseMageEnemy : MonoBehaviour
{
    public EnemyMageStateMachine stateMachine;
    public EnemyMageState patrolState;
    public EnemyMageState searchState;
    public EnemyMageState pursuitState;
    public EnemyMageState attackState;
    
    public Animator animationController;
    
    public Sight sight_sensor_;
    public NavMeshAgent agent_;
    
    public GameObject fireEffect;
    
    //Patrolling — points are generated at runtime (base scene waypoints + detection clusters), so they are
    //world positions, not Transforms. Seeded by MageSpawner.SpawnMage and grown by AddPatrolArea.
    public List<Vector3> patrolPoints = new List<Vector3>();
    [HideInInspector] public Vector3 currentTarget;
    [HideInInspector] public bool hasTarget = false;
    private int index = 0;
    private bool isMovingToWaypoint = false;
    public float patrolWaitTime = 2.0f;

    //Dynamic patrol: each scout detection scatters this many NavMesh-sampled points within this radius
    //around where the player was spotted, so the mage concentrates its patrol on hot zones.
    public float detectionClusterRadius = 6f;
    public int detectionClusterCount = 4;

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

    //Detection memory: keep pursuing the last known position for this long after losing line of sight, so a single-frame occlusion (corner, lag, momentary cover) does not make the mage give up.
    public float loseSightGracePeriod = 3.5f;
    [HideInInspector] public float lastSeenTime = -Mathf.Infinity;
    
    [SerializeField] public bool isBeingSeen;

    //Visibility-based speed — fast while the player is watching, slow while not. Pursuit overrides both with pursuitSpeed so the chase feels urgent.
    public float hiddenSpeed = 1.5f;
    public float visibleSpeed = 3.0f;
    public float pursuitSpeed = 4.5f;
    private DitherVisibility ditherVisibility_;

    private void Awake()
    {
        agent_ = GetComponent<NavMeshAgent>();
        ditherVisibility_ = GetComponentInChildren<DitherVisibility>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireEffect.SetActive(false);
        
        if (patrolPoints.Count > 0)
        {
            //Set first target
            currentTarget = patrolPoints[index];
            hasTarget = true;

            //Start moving towards first target
            agent_.SetDestination(currentTarget);
        }
        
        //Initialize state machine
        stateMachine = new EnemyMageStateMachine();
        
        //Create state instances
        patrolState = new EnemyMagePatrolState(this, stateMachine, animationController, "Patrol");
        searchState = new EnemyMageSearchState(this, stateMachine, animationController, "Search");
        pursuitState = new EnemyMagePursuitState(this, stateMachine, animationController, "Pursuit");
        attackState = new EnemyMageAttackState(this, stateMachine, animationController, "Attack");

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
        if (stateMachine != null && stateMachine._CurrentState is EnemyMagePursuitState)
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

    public IEnumerator MoveToNextWaypoint()
    {
        //if moving, don't call again
        if (isMovingToWaypoint)
            yield break;

        isMovingToWaypoint = true;

        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            hasTarget = false;
            isMovingToWaypoint = false;
            yield break;
        }

        //Pause movement at each waypoint
        agent_.isStopped = true;
        yield return new WaitForSeconds(patrolWaitTime);

        //Clamp waypoint index
        index = Mathf.Clamp(index, 0, patrolPoints.Count - 1);

        index++;

        //Loop if reached the last waypoint
        if (index >= patrolPoints.Count)
            index = 0;

        currentTarget = patrolPoints[index];
        hasTarget = true;

        //Move to next waypoint
        agent_.isStopped = false;
        agent_.SetDestination(currentTarget);

        isMovingToWaypoint = false;
    }
    
    public void FindNearestWaypoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            hasTarget = false;
            return;
        }

        int nearest = 0;
        float nearestDistance = float.MaxValue;

        //Check distance of each patrol point, find the nearest
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, patrolPoints[i]);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = i;
            }
        }

        index = nearest;
        currentTarget = patrolPoints[index];
        hasTarget = true;
        agent_.isStopped = false;
        agent_.SetDestination(currentTarget);
    }

    //Scatters `count` NavMesh-sampled patrol points within `radius` of `center` and appends them, so each
    //scout detection piles a fresh hot-zone onto the mage's patrol. Called by MageSpawner.SpawnMage.
    public void AddPatrolArea(Vector3 center, float radius, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 candidate = center + UnityEngine.Random.insideUnitSphere * radius;
            candidate.y = center.y;
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, radius, NavMesh.AllAreas))
                patrolPoints.Add(hit.position);
        }
    }
}
