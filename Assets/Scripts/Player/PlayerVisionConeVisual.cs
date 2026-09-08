using System.Collections.Generic;
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

    [Header("Silhouette")]
    [Tooltip("Distance gap between two neighbouring rays that counts as an occluder edge rather than a slope.")]
    public float edgeDistanceThreshold = 0.35f;

    [Tooltip("Bisection steps used to pin down each occluder edge. 0 disables refinement.")]
    [Range(0, 10)]
    public int edgeRefineSteps = 6;

    private Mesh _coneMesh;
    private MeshFilter _meshFilter;

    // Reused every frame so the cone allocates nothing once it is running.
    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<int> _triangles = new List<int>();

    // Same Mesh instance is reused every frame (Clear + reassign), so a fog-of-war revealer can share
    // this reference once and stay in sync automatically — the cone already respects wall occlusion.
    public Mesh ConeMesh => _coneMesh;

    // One ray of the fan: the angle it was cast at and how far it got.
    private struct Sample
    {
        public float Angle;
        public float Distance;
    }

    private void Awake()
    {
        if (visionCone == null)
            visionCone = GetComponentInParent<PlayerVisionCone>();

        _meshFilter = GetComponent<MeshFilter>();
        _coneMesh = new Mesh { name = "PlayerVisionCone" };
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
        int rays = Mathf.Max(2, resolution);

        _vertices.Clear();
        _triangles.Clear();
        _vertices.Add(Vector3.zero);

        float half = angle * 0.5f;
        float angleIncrement = angle / (rays - 1);

        Sample previous = Cast(-half, range, obstructionMask);
        _vertices.Add(ToLocal(previous));

        for (int i = 1; i < rays; i++)
        {
            Sample current = Cast(-half + angleIncrement * i, range, obstructionMask);

            // A depth jump between neighbouring rays is an occluder edge, not a slope. Left alone the
            // fan stretches one triangle across the whole gap and the cut reads as a diagonal smear
            // that drifts by up to a full angular step. Bisect the gap to find where the edge actually
            // sits, then emit both of its sides so the silhouette comes out straight.
            if (Mathf.Abs(current.Distance - previous.Distance) > edgeDistanceThreshold)
            {
                Sample near = previous;
                Sample far = current;

                for (int step = 0; step < edgeRefineSteps; step++)
                {
                    Sample mid = Cast((near.Angle + far.Angle) * 0.5f, range, obstructionMask);

                    // Replace whichever side the midpoint belongs to, so the pair keeps straddling
                    // the jump and stays ordered by angle.
                    if (Mathf.Abs(mid.Distance - near.Distance) <= Mathf.Abs(mid.Distance - far.Distance))
                        near = mid;
                    else
                        far = mid;
                }

                _vertices.Add(ToLocal(near));
                _vertices.Add(ToLocal(far));
            }

            _vertices.Add(ToLocal(current));
            previous = current;
        }

        for (int i = 1; i < _vertices.Count - 1; i++)
        {
            _triangles.Add(0);
            _triangles.Add(i);
            _triangles.Add(i + 1);
        }

        _coneMesh.Clear();
        _coneMesh.SetVertices(_vertices);
        _coneMesh.SetTriangles(_triangles, 0);
        _coneMesh.RecalculateNormals();
    }

    private Sample Cast(float angle, float range, LayerMask obstructionMask)
    {
        float sine = Mathf.Sin(angle);
        float cosine = Mathf.Cos(angle);

        // Raycast in world space; the mesh vertex is built in local space by ToLocal.
        Vector3 rayDirection = (transform.forward * cosine) + (transform.right * sine);

        // Ignore triggers, exactly like PlayerVisionCone.ScanCone. Pickup radii, room volumes and
        // pressure plates block nothing, so they must not carve notches out of the drawn cone either.
        bool blocked = Physics.Raycast(transform.position, rayDirection, out RaycastHit hit,
                                       range, obstructionMask, QueryTriggerInteraction.Ignore);

        return new Sample { Angle = angle, Distance = blocked ? hit.distance : range };
    }

    // The ray travels in WORLD space, so its distance is in world metres - but the vertex it produces
    // is stored in LOCAL space, where the transform's scale gets applied on the way back out. The player
    // root is scaled (0.7, 1, 0.7), so a hit 5.26m away was being drawn at 5.26 x 0.7 = 3.68m and the
    // cone stopped a third short of where it actually sees. Enemies never showed it: their scale is 1.
    //
    // Round-trip through the transform instead of assuming local and world agree. This is exact for any
    // scale or rotation, and a no-op when the scale is 1.
    private Vector3 ToLocal(Sample sample)
    {
        Vector3 worldDirection = (transform.forward * Mathf.Cos(sample.Angle)) + (transform.right * Mathf.Sin(sample.Angle));
        return transform.InverseTransformPoint(transform.position + worldDirection * sample.Distance);
    }

    // Optional: visualize cone in editor
    private void OnDrawGizmosSelected()
    {
        if (visionCone == null)
            return;

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, visionCone.visionRadius);
    }
}
