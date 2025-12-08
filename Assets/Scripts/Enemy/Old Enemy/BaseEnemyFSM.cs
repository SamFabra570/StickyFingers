using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class BaseEnemyFSM : MonoBehaviour
{
    public enum MindStates
    {
        kWait,
        kPatrol,
        kPursuit,
        kAttack,
        kSearch
    }
    
    public MindStates current_mind_state_;
    public Sight sight_sensor_;
    public NavMeshAgent agent_;

    public float attack_distance_ = 2.0f;

    public float stop_attack_distance_multiplier = 1.2f;

    //public float stun_time_ = 2.0f;

    public GameObject fireEffect;
    
    //Patrolling
    public List<Transform> waypoints;
    private Transform currentTarget;
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
    private float searchEndTime;
    private Vector3 forwardPoint;

    private void Awake()
    {
        agent_ = GetComponentInParent<NavMeshAgent>();
    }

    // Start is called before the first frame update
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
    }

    // Update is called once per frame
    void Update()
    {
        if(current_mind_state_ == MindStates.kWait)
        {
            MindWait();
        }else if(current_mind_state_ == MindStates.kPatrol)
        {
            MindPatrol();
        }else if(current_mind_state_ == MindStates.kPursuit)
        {
            MindPursuit();
        }else if(current_mind_state_ == MindStates.kAttack)
        {
            MindAttack();
        }else if(current_mind_state_ == MindStates.kSearch)
        {
            MindSearch();
        }

        
    }

    IEnumerator MoveToNextWaypoint()
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
    
    void MindWait()
    {
        BodyWait();
        Debug.Log("Waiting");
        current_mind_state_ = MindStates.kPatrol;

    }
    
    void MindPatrol()
    {
        BodyPatrol();

        if(sight_sensor_.detected_object_ != null)
        {
            current_mind_state_ = MindStates.kPursuit;
        }

    }
    void MindPursuit()
    {
        BodyPursuit();
        //Debug.Log("Following");

        if(sight_sensor_.detected_object_ == null)
        {
            
            current_mind_state_ = MindStates.kSearch;
            return;
        }

        float distance_to_target = Vector3.Distance(transform.position, sight_sensor_.detected_object_.transform.position);
        if(distance_to_target <= attack_distance_)
        {
            Debug.Log("switching to attack");
            current_mind_state_ = MindStates.kAttack;
        }
        
    }
    
    void MindAttack()
    {
        Debug.Log("Attacking");
        BodyAttack();
        

        if (sight_sensor_.detected_object_ == null)
        {
            current_mind_state_ = MindStates.kWait;
            return;
        }

        float distance_to_target = Vector3.Distance(transform.position, sight_sensor_.detected_object_.transform.position);
        if (distance_to_target > attack_distance_ * stop_attack_distance_multiplier)
        {
            current_mind_state_ = MindStates.kWait;
        }
    }

    void MindSearch()
    {
        //Set end of search time
        if (!isSearching)
        {
            searchEndTime = Time.time + searchTime;
            isSearching = true;
        }
        
        if (sight_sensor_.detected_object_ != null)
        {
            current_mind_state_ = MindStates.kPursuit;
            isSearching = false;
            return;
        }
        
        BodySearch();
        
        //If search time has ended, go back to patrol
        if (Time.time > searchEndTime)
        {
            isSearching = false;
            FindNearestWaypoint();
            fireEffect.SetActive(false);
            current_mind_state_ = MindStates.kPatrol;
        }
    }

    void BodyWait()
    {
        agent_.isStopped = true;
    }
    
    void BodyPatrol()
    {
        Debug.Log("Starting Patrol");
        //If in patrol state, start following waypoints
        if (current_mind_state_ == MindStates.kPatrol && agent_.isStopped)
        {
            agent_.isStopped = false;
            StartCoroutine(MoveToNextWaypoint());
        }
        
        if (currentTarget != null)
        {
            //Check if agent has arrived at target position
            if ((Vector3.Distance(transform.position, currentTarget.position) <= 2f) && isMoving)
            {
                isMoving = false;
                StartCoroutine("MoveToNextWaypoint");
            }
        }
        
    }
    
    void BodyPursuit()
    {
        if (agent_ != null && sight_sensor_.detected_object_ != null)
        {
            agent_.isStopped = false;
            fireEffect.SetActive(true);
            agent_.SetDestination(sight_sensor_.detected_object_.transform.position);
        }
           
    }
    
    void BodyAttack()
    {
        Vector3 dir = Vector3.Normalize(transform.forward);
                

        agent_.isStopped = true;
        //atacar o algo
        //sight_sensor_.detected_object_.attachedRigidbody.AddForce(dir * 1500);
    }

    void BodySearch()
    {
        agent_.isStopped = false;

        Debug.Log("Searching");
        forwardPoint = agent_.transform.position + transform.forward * searchDistance;
        agent_.SetDestination(forwardPoint);
    }

    void FindNearestWaypoint()
    {
        Transform nearestWaypoint = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < waypoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].position);
            
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestWaypoint = waypoints[i];
            }
        }
        
        if (nearestWaypoint != null)
        {
            agent_.isStopped = false;
            agent_.SetDestination(nearestWaypoint.position);
            currentTarget = nearestWaypoint;
            index = waypoints.IndexOf(nearestWaypoint);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attack_distance_);

    }

    
}
