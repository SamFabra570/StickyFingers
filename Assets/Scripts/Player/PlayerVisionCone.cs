using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerVisionCone : MonoBehaviour
{
    [Header("Cone Settings")]
    [Range(0f, 360f)]
    public float visionAngle = 90f;
    public float visionRadius = 10f;

    [Tooltip("Proximity reveal: targets this close are revealed regardless of cone angle (still blocked by walls). Set 0 to disable.")]
    public float peripheralRadius = 2f;

    [Header("Occlusion")]
    [FormerlySerializedAs("occlusionMask")]
    [Tooltip("Layers that BLOCK DETECTION. Real walls and any geometry that should hide enemies/objects from the cone reveal.")]
    public LayerMask detectionMask;   // walls, environment geometry

    [Tooltip("Layers that CUT THE VISUAL CONE MESH. Usually the same as Detection Mask. Put invisible decorative colliders OUTSIDE this mask so the cone doesn't bite around chandeliers/props. If left empty (Nothing), falls back to Detection Mask.")]
    public LayerMask visualMask;

    public LayerMask targetMask;      // enemies + stealable objects

    //Resolved mask used by the visual cone — falls back to detectionMask when visualMask is not set, so existing scenes keep working.
    public LayerMask ResolvedVisualMask => visualMask.value != 0 ? visualMask : detectionMask;

    [Header("Performance")]
    public float scanInterval = 0.1f; // seconds between scans

    // All DitherVisibility objects currently inside the cone
    private HashSet<DitherVisibility> _visibleNow = new HashSet<DitherVisibility>();
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= scanInterval)
        {
            _timer = 0f;
            ScanCone();
        }
    }

    private void ScanCone()
    {
        // Collect all targets within the radius
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRadius, targetMask);

        HashSet<DitherVisibility> visibleThisFrame = new HashSet<DitherVisibility>();

        foreach (Collider col in hits)
        {
            DitherVisibility dv = col.GetComponentInParent<DitherVisibility>();
            if (dv == null) continue;

            Vector3 dirToTarget = (col.bounds.center - transform.position);
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            // Inside the cone angle, OR close enough to reveal peripherally (peripheralRadius = 0 disables it)
            bool insideCone       = angle <= visionAngle * 0.5f;
            bool insidePeripheral = dirToTarget.magnitude <= peripheralRadius;
            if (!insideCone && !insidePeripheral) continue;

            // Raycast for occlusion
            if (Physics.Raycast(transform.position, dirToTarget.normalized,
                                 out RaycastHit hit, visionRadius, detectionMask | targetMask))
            {
                // Make sure we hit the target, not a wall first
                if (hit.collider != col) continue;
            }

            visibleThisFrame.Add(dv);
        }

        // Targets that just entered the cone
        foreach (DitherVisibility dv in visibleThisFrame)
        {
            if (!_visibleNow.Contains(dv))
                dv.SetVisible(true);
        }

        // Targets that just left the cone
        foreach (DitherVisibility dv in _visibleNow)
        {
            if (!visibleThisFrame.Contains(dv))
                dv.SetVisible(false);
        }

        _visibleNow = visibleThisFrame;
    }

    // Optional: visualize cone in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Vector3 leftBound  = Quaternion.Euler(0, -visionAngle * 0.5f, 0) * transform.forward;
        Vector3 rightBound = Quaternion.Euler(0,  visionAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftBound  * visionRadius);
        Gizmos.DrawRay(transform.position, rightBound * visionRadius);
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
