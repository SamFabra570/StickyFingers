using UnityEngine;

/// <summary>
/// Procedural mesh that visualises the player's vision cone.
/// Reads its dimensions from PlayerVisionCone so the drawn cone always
/// matches what the detection logic actually scans — no drift.
/// </summary>
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class PlayerVisionConeVisual : MonoBehaviour
{
    [Tooltip("Detection component this cone mirrors. Auto-found in parents if left empty.")]
    public PlayerVisionCone visionCone;

    [Tooltip("Triangle count of the cone fan — higher = smoother edge.")]
    public int resolution = 120;

    private Mesh _coneMesh;
    private MeshFilter _meshFilter;

    // Same Mesh instance is reused every frame (Clear + reassign), so a fog-of-war revealer can share
    // this reference once and stay in sync automatically — the cone already respects wall occlusion.
    public Mesh ConeMesh => _coneMesh;

    private void Awake()
    {
        if (visionCone == null)
            visionCone = GetComponentInParent<PlayerVisionCone>();

        _meshFilter = GetComponent<MeshFilter>();
        _coneMesh = new Mesh();
        _meshFilter.mesh = _coneMesh;
    }

    private void Update()
    {
        if (visionCone == null)
            return;

        // visionAngle is stored in degrees on PlayerVisionCone — convert here. Uses the visual-specific mask (falls back to detection mask if unset) so decorative invisible colliders can be excluded without affecting detection.
        DrawCone(visionCone.visionRadius, visionCone.visionAngle * Mathf.Deg2Rad, visionCone.ResolvedVisualMask);
    }

    private void DrawCone(float range, float angle, LayerMask obstructionMask)
    {
        int[] triangles = new int[(resolution - 1) * 3];
        Vector3[] vertices = new Vector3[resolution + 1];
        vertices[0] = Vector3.zero;

        float currentAngle = -angle / 2f;
        float angleIncrement = angle / (resolution - 1);

        for (int i = 0; i < resolution; i++)
        {
            float sine = Mathf.Sin(currentAngle);
            float cosine = Mathf.Cos(currentAngle);

            // Raycast in world space, but build the mesh vertex in local space.
            Vector3 rayDirection  = (transform.forward * cosine) + (transform.right * sine);
            Vector3 vertDirection = (Vector3.forward * cosine) + (Vector3.right * sine);

            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, range, obstructionMask))
                vertices[i + 1] = vertDirection * hit.distance;
            else
                vertices[i + 1] = vertDirection * range;

            currentAngle += angleIncrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i]     = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        _coneMesh.Clear();
        _coneMesh.vertices = vertices;
        _coneMesh.triangles = triangles;
        _coneMesh.RecalculateNormals();
    }
}
