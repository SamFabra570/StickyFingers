using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public float distance_;
    public float height_;
    public LayerMask sensor_layer_;
    public LayerMask sensor_layer_2;
    public LayerMask obstacles_layer_;
    
    public Collider detected_object_;

    //new vision cone
   //the vision cone will be made up of triangles, the higher this value is the pretier the vision cone will be
    private void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance_, sensor_layer_ | sensor_layer_2);

        detected_object_ = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider single_collider = colliders[i];

            PlayerController player = single_collider.GetComponent<PlayerController>();

            if (player != null && player.isInvisible)
                continue;

            // Hearing is omnidirectional — no angle check. But a wall between us muffles it out.
            if (Physics.Linecast(transform.position, single_collider.bounds.center, out RaycastHit hit, obstacles_layer_))
            {
                Debug.DrawLine(transform.position, hit.point, Color.green); // wall blocks the sound
                continue;
            }

            Debug.DrawLine(transform.position, single_collider.bounds.center, Color.red); // heard
            detected_object_ = single_collider;
            break;
        }

    }

    private void OnDrawGizmos()
    {
        // Hearing is omnidirectional — the radius is all that matters now.
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distance_);
    }


    
}
