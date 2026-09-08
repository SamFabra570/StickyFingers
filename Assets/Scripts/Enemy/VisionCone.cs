using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VisionCone : MonoBehaviour
{
    public float VisionRange;
    public float VisionAngle;
    public LayerMask VisionObstructingLayer;
    public int VisionConeResolution = 120;

    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;

    void Awake()
    {
        MeshFilter_ = GetComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        MeshFilter_.mesh = VisionConeMesh;
        VisionAngle *= Mathf.Deg2Rad;
    }

    
    void Update()
    {
        DrawVisionCone();//calling the vision cone function everyframe just so the cone is updated every frame
    }

    void DrawVisionCone()//this method creates the vision cone mesh
    {
	int[] triangles = new int[(VisionConeResolution - 1) * 3];
    	Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -VisionAngle /2;
        float angleIcrement = VisionAngle / (VisionConeResolution - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < VisionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            //The ray measures world metres but the vertex is stored in local space, where the transform's
            //scale is applied on the way out. Round-trip through the transform so the drawn edge lands on
            //the point the ray actually hit. No-op at scale 1 (every enemy today), but the player cone had
            //exactly this bug and drew at 70% of its real reach because its root is scaled 0.7.
            float reach = Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer)
                ? hit.distance
                : VisionRange;
            Vertices[i + 1] = transform.InverseTransformPoint(transform.position + RaycastDirection * reach);


            Currentangle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;
    }


}
