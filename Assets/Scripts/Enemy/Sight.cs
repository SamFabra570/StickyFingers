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
    
    public Collider detected_object_;

    //new vision cone
   //the vision cone will be made up of triangles, the higher this value is the pretier the vision cone will be
    private void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
        //meshObject.transform.rotation = transform.rotation;
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance_, sensor_layer_ | sensor_layer_2);

        detected_object_ = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider single_collider = colliders[i];

            PlayerController player = single_collider.GetComponent<PlayerController>();

            if (player != null && player.isInvisible)
                continue;

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
