using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyFSM : MonoBehaviour
{
    public enum MindStates
    {
        kWait,
        kSeek,
        kPursuit,
        kAttack,
        kFlee
    }
   
    public MindStates current_mind_state_;
    public Sight sight_sensor_;
    public NavMeshAgent agent_;

    public float attack_distance_ = 2.0f;

    public float stop_attack_distance_multiplier = 1.2f;

    public float stun_time_ = 2.0f;


    private void Awake()
    {
        agent_ = GetComponentInParent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(current_mind_state_ == MindStates.kWait)
        {
            MindWait();
        }else if(current_mind_state_ == MindStates.kSeek)
        {
            MindSeek();
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
    
    void MindWait()
    {
        BodyWait();
        Debug.Log("Waiting");
        //a�adir corrutina stun time
        current_mind_state_ = MindStates.kSeek;

    }
    void MindSeek()
    {
        BodySeek();
        Debug.Log("Seeking");

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
    void BodySeek()
    {
        //rotate o algo
        if (current_mind_state_ == MindStates.kSeek)
        {
            Debug.Log("trying to turnnnnn");
            transform.Rotate(new Vector3(0, 10, 0));

        }
        
    }
    void BodyPursuit()
    {
        if (agent_ != null && sight_sensor_.detected_object_ != null)
        {
            agent_.isStopped = false;
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
