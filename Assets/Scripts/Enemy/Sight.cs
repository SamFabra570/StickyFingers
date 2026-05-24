using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Sight : MonoBehaviour
{

    public float distance_;
    public float angle_;
    public float height_;
    public LayerMask sensor_layer_;
    public LayerMask sensor_layer_2;
    public LayerMask obstacles_layer_;

    [Tooltip("Proximity sense: detects targets this close regardless of cone angle (still blocked by walls). Set 0 to disable.")]
    public float peripheral_radius_ = 2f;

    public Collider detected_object_;

    private bool _wasSeeingPlayer;   // tracks player-detection transitions for the detection vignette

    //new vision cone
   //the vision cone will be made up of triangles, the higher this value is the prettier the vision cone will be
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

            // single_collider may be a child collider (wings/ability hitboxes share the Player layer),
            // so walk up to the root to honor invisibility no matter which collider we hit.
            PlayerController player = single_collider.GetComponentInParent<PlayerController>();

            if (player != null && player.isInvisible)
                continue;

            Vector3 dir_to_collider = Vector3.Normalize(single_collider.bounds.center - transform.position);

            // Angle -> coste alto / alternativa Dot
            float angle_to_collider = Vector3.Angle(transform.forward, dir_to_collider);
            float distance_to_collider = Vector3.Distance(transform.position, single_collider.bounds.center);

            // Inside the vision cone, OR close enough to be sensed peripherally (peripheral_radius_ = 0 disables it)
            bool insideCone       = angle_to_collider < angle_;
            bool insidePeripheral = distance_to_collider <= peripheral_radius_;
            if (!insideCone && !insidePeripheral)
                continue;

            // OCCLUSION: a wall between us blocks detection. But the player's OWN colliders (SoundGenerator,
            // ability hitboxes) live on the obstacles layer and surround the player — they must NOT count as
            // a wall, otherwise the player occludes themselves. Only a real wall (not part of the player) blocks.
            if (Physics.Linecast(transform.position, single_collider.bounds.center, out RaycastHit hit, obstacles_layer_)
                && hit.collider.GetComponentInParent<PlayerController>() == null)
            {
                Debug.DrawLine(transform.position, hit.point, Color.green); // blocked by a real wall
                continue;
            }

            // Clear line of sight → detected
            Debug.DrawLine(transform.position, single_collider.bounds.center, Color.red);
            detected_object_ = single_collider;
            break;
        }

        ReportDetection(detected_object_ != null
            && detected_object_.GetComponentInParent<PlayerController>() != null);
    }

    //Notifies the player when this sensor starts/stops seeing them (reference-counted on PlayerController).
    private void ReportDetection(bool seeingPlayer)
    {
        if (seeingPlayer == _wasSeeingPlayer) return;
        _wasSeeingPlayer = seeingPlayer;

        if (PlayerController.Instance == null) return;
        if (seeingPlayer) PlayerController.Instance.AddDetector();
        else              PlayerController.Instance.RemoveDetector();
    }

    private void OnDisable()
    {
        //If disabled while seeing the player (e.g. scout despawns), release our count so it doesn't stick.
        if (_wasSeeingPlayer && PlayerController.Instance != null)
            PlayerController.Instance.RemoveDetector();
        _wasSeeingPlayer = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, distance_);

        Vector3 right_dir = Quaternion.Euler(0.0f, angle_, 0.0f) * transform.forward;
        Gizmos.DrawRay(transform.position, right_dir * distance_);

        Vector3 left_dir = Quaternion.Euler(0.0f, -angle_, 0.0f) * transform.forward;
        Gizmos.DrawRay(transform.position, left_dir * distance_);

        /*if (mesh)
        {
            Gizmos.color = meshColor_;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
            //meshObject.GetComponent<MeshFilter>().mesh = mesh;
        }*/
    }

    private Mesh CreateWedgemesh()
    {
        
        Mesh mesh = new Mesh();
        int segments = 10;
        int numTriangles = (segments *4)+2+2;
        int numVertices = numTriangles * 3;
        
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];
        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0.0f, -angle_, 0.0f) * transform.forward*distance_;
        Vector3 bottomRight = Quaternion.Euler(0.0f, angle_, 0.0f) * transform.forward*distance_;
        
        Vector3 topCenter = bottomCenter +Vector3.up * height_;
        Vector3 topLeft = bottomLeft +Vector3.up * height_;
        Vector3 topRight = bottomRight +Vector3.up * height_;
        
        int vert=0;
        
        //left Side 
        
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;
        
        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;
        
        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;
        
        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;
        
        float currentAngle = -angle_;
        float deltaAngle = (angle_ *2)/segments;
        for (int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0.0f, currentAngle, 0.0f) * transform.forward*distance_;
            bottomRight = Quaternion.Euler(0.0f, currentAngle+deltaAngle, 0.0f) * transform.forward*distance_;
        
            topLeft = bottomLeft +Vector3.up * height_;
            topRight = bottomRight +Vector3.up * height_;
            currentAngle += deltaAngle;
            //far side
        
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;
        
            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
        
            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;
        
            //bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;
        }
        

        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    

}
