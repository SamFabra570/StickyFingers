using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Diagnostic for "why is the cone cut here?". Casts the same fan PlayerVisionConeVisual draws
/// and reports which colliders are actually stopping the rays, so the offending objects can be
/// found by name instead of by guesswork. Put it on the same object as the cone visual.
/// </summary>
[RequireComponent(typeof(PlayerVisionConeVisual))]
public class VisionConeDebug : MonoBehaviour
{
    [Tooltip("Press this during Play to dump the report to the Console.")]
    public KeyCode reportKey = KeyCode.F9;

    [Tooltip("Keep drawing the blocked rays in the Scene view every frame.")]
    public bool drawRays = true;

    private PlayerVisionCone _cone;
    private PlayerVisionConeVisual _visual;
    private readonly List<Vector3> _blockedAt = new List<Vector3>();

    private void Awake()
    {
        _visual = GetComponent<PlayerVisionConeVisual>();
        _cone = _visual.visionCone != null ? _visual.visionCone : GetComponentInParent<PlayerVisionCone>();
    }

    private void Update()
    {
        if (_cone == null) return;

        if (drawRays) Sample(null);
        if (Input.GetKeyDown(reportKey)) Report();
    }

    private void Report()
    {
        var tally = new Dictionary<Collider, int>();
        Sample(tally);

        if (tally.Count == 0)
        {
            Debug.Log("[VisionConeDebug] Nothing is cutting the cone right now.");
            return;
        }

        var lines = new List<string>();
        foreach (var pair in tally)
        {
            Collider c = pair.Key;
            lines.Add($"{pair.Value,4} rayos | capa {LayerMask.LayerToName(c.gameObject.layer)} " +
                      $"| {Path(c.transform)}{(c.isTrigger ? "  [TRIGGER]" : "")}");
        }
        lines.Sort((a, b) => b.CompareTo(a));

        Debug.Log($"[VisionConeDebug] {tally.Count} colliders estan cortando el cono:\n" +
                  string.Join("\n", lines) +
                  "\n\nTodo lo que sea mobiliario -> capa IgnoreVision. Solo las paredes se quedan en Default.");
    }

    // Mirrors PlayerVisionConeVisual.DrawCone exactly: same origin, same mask, same trigger rule.
    private void Sample(Dictionary<Collider, int> tally)
    {
        _blockedAt.Clear();

        int rays = Mathf.Max(2, _visual.resolution);
        float angle = _cone.visionAngle * Mathf.Deg2Rad;
        float half = angle * 0.5f;
        float increment = angle / (rays - 1);
        LayerMask mask = _cone.ResolvedVisualMask;

        for (int i = 0; i < rays; i++)
        {
            float a = -half + increment * i;
            Vector3 dir = (transform.forward * Mathf.Cos(a)) + (transform.right * Mathf.Sin(a));

            if (!Physics.Raycast(transform.position, dir, out RaycastHit hit,
                                 _cone.visionRadius, mask, QueryTriggerInteraction.Ignore))
                continue;

            _blockedAt.Add(hit.point);
            if (tally == null) continue;
            tally.TryGetValue(hit.collider, out int n);
            tally[hit.collider] = n + 1;
        }
    }

    private static string Path(Transform t)
    {
        string path = t.name;
        for (Transform p = t.parent; p != null; p = p.parent)
            path = p.name + "/" + path;
        return path;
    }

    private void OnDrawGizmos()
    {
        if (!drawRays) return;
        Gizmos.color = Color.red;
        foreach (Vector3 p in _blockedAt)
        {
            Gizmos.DrawLine(transform.position, p);
            Gizmos.DrawSphere(p, 0.08f);
        }
    }
}
