using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{

    public float distance_;
    public float angle_;
    public LayerMask sensor_layer_;
    public LayerMask obstacles_layer_;

    public Collider detected_object_;

    // Start is called before the first frame update
    void Start()
    {
       



    }


    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance_, sensor_layer_);

        detected_object_ = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider single_collider = colliders[i];

            Vector3 dir_to_collider = Vector3.Normalize(single_collider.bounds.center - transform.position);

            // Angle -> coste alto / alternativa Dot
            float angle_to_collider = Vector3.Angle(transform.forward, dir_to_collider);

            if(angle_to_collider < angle_)
            {
                if(!Physics.Linecast(transform.position, single_collider.bounds.center, out RaycastHit hit,  obstacles_layer_))
                {
                    Debug.DrawLine(transform.position, single_collider.bounds.center, Color.red);
                    detected_object_ = single_collider;
                    break;
                }
                else
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, distance_);

        Vector3 right_dir = Quaternion.Euler(0.0f, angle_, 0.0f) * transform.forward;
        Gizmos.DrawRay(transform.position, right_dir * distance_);

        Vector3 left_dir = Quaternion.Euler(0.0f, -angle_, 0.0f) * transform.forward;
        Gizmos.DrawRay(transform.position, left_dir * distance_);
    }

    private void OnDrawGizmosSelected()
    {
        
    }
}
