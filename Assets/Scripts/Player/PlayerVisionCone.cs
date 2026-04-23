using System.Collections.Generic;
using UnityEngine;

public class PlayerVisionCone : MonoBehaviour
{
    [Header("Cone Settings")]
    [Range(0f, 360f)]
    public float visionAngle = 90f;
    public float visionRadius = 10f;

    [Header("Occlusion")]
    public LayerMask occlusionMask;   // walls, environment geometry
    public LayerMask targetMask;      // enemies + stealable objects

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

            if (angle > visionAngle * 0.5f) continue; // outside cone angle

            // Raycast for occlusion
            if (Physics.Raycast(transform.position, dirToTarget.normalized,
                                 out RaycastHit hit, visionRadius, occlusionMask | targetMask))
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
