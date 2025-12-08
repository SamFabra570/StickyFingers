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
    public EnemyState pursuitState;
    public EnemyState attackState;
    
    public Animator animationController;
    
    public Sight sight_sensor_;
    public NavMeshAgent agent_;
    
    public GameObject fireEffect;
    
    //Patrolling
    public List<Transform> waypoints;
    public Transform currentTarget;
    private int index = 0;
    private bool isMoving = true;
    private bool atEnd = false;
    private bool isReversing = false;
    public float patrolWaitTime = 2.0f;

    //Searching for player
    private Vector3 searchDir;
    private bool isSearching = false;
    public float searchDistance = 5.0f;
    public float searchTime = 10.0f;
    private Vector3 forwardPoint;
    
    //Attacking
    public float attack_distance_ = 2.0f;
    public float stop_attack_distance_multiplier = 1.2f;

    private void Awake()
    {
        agent_ = GetComponentInParent<NavMeshAgent>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireEffect.SetActive(false);
        
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
        pursuitState = new EnemyPursuitState(this, stateMachine, animationController, "Pursuit");
        attackState = new EnemyAttackState(this, stateMachine, animationController, "Attack");

        //Start patrol state
        stateMachine.InitializeStateMachine(patrolState);
    }

    private void Update()
    {
        if (stateMachine._CurrentState != null)
            stateMachine._CurrentState.LogicUpdate();
    }
    
    void FixedUpdate()
    {
        if (stateMachine._CurrentState != null)
            stateMachine._CurrentState.PhysicsUpdate();
    }

    public IEnumerator MoveToNextWaypoint()
    {
        if (!isReversing)
        {
            index++;
        }
        
        //If not at last waypoint and not in reverse
        if (index < waypoints.Count && !isReversing)
        {
            //Pause for set seconds before going to first waypoint
            if (index == 1 &&!isSearching)
                yield return new WaitForSeconds(patrolWaitTime);
            
            currentTarget = waypoints[index];
        }
        else
        {
            //If at last waypoint pause for set seconds
            if (!atEnd)
            {
                atEnd = true;
                yield return new WaitForSeconds(patrolWaitTime);
            }

            index--;
            isReversing = true;

            //If next waypoint is the first point, reset isReversing and atEnd
            if (index == 0)
            {
                isReversing = false;
                atEnd = false;
            }
            
            currentTarget = waypoints[index];
        }
        
        //Move to next waypoint
        agent_.SetDestination(currentTarget.position);
        isMoving = true;
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
}
