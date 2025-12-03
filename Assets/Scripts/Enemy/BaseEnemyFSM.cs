using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyFSM : MonoBehaviour
{
    public enum MindStates
    {
        kWait,
        kPatrol,
        kPursuit,
        kAttack,
        kFlee
    }
    
    public MindStates current_mind_state_;
    public Sight sight_sensor_;
    public NavMeshAgent agent_;

    public float attack_distance_ = 2.0f;

    public float stop_attack_distance_multiplier = 1.2f;

    //public float stun_time_ = 2.0f;

    public GameObject fireEffect;
    
    public List<Transform> waypoints;
    private Transform currentTarget;
    private int index = 0;

    private bool isMoving = true;
    private bool atEnd = false;
    private bool isReversing = false;

    public float waitTime = 2.0f;

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
        }else if(current_mind_state_ == MindStates.kFlee)
        {
            MindFlee();
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
            if (index == 1)
                yield return new WaitForSeconds(waitTime);
            
            currentTarget = waypoints[index];
        }
        else
        {
            //If at last waypoint pause for set seconds
            if (!atEnd)
            {
                atEnd = true;
                yield return new WaitForSeconds(waitTime);
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
        Debug.Log("Starting Patrol");

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
            fireEffect.SetActive(false);
            current_mind_state_ = MindStates.kWait;
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
    
    void MindFlee()
    {
        BodyFlee();
        Debug.Log("Fleeing");
    }

    void BodyWait()
    {
        agent_.isStopped = true;
    }
    
    void BodyPatrol()
    {
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
    
    void BodyFlee()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attack_distance_);

    }

    
}
