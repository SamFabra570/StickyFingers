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

        if (drawRays) Sample(null, 0.0f);
        if (Input.GetKeyDown(reportKey)) Report();
    }

    // What the rays that died on one collider have in common. A collider is only half the answer:
    // sixty rays dying at 1.5m on a mesh that reaches 3.4m high is furniture INSIDE the wall mesh,
    // and no layer change can separate those two - they are one collider. The distances say which.
    private struct Deaths
    {
        public int Count;
        public float MinDistance, MaxDistance;
        public float MinHeight, MaxHeight;

        public void Add(float distance, float height)
        {
            if (Count == 0)
            {
                MinDistance = MaxDistance = distance;
                MinHeight = MaxHeight = height;
            }
            else
            {
                MinDistance = Mathf.Min(MinDistance, distance);
                MaxDistance = Mathf.Max(MaxDistance, distance);
                MinHeight = Mathf.Min(MinHeight, height);
                MaxHeight = Mathf.Max(MaxHeight, height);
            }
            Count++;
        }
    }

    private void Report()
    {
        var tally = new Dictionary<Collider, Deaths>();
        Sample(tally, 0.0f);

        if (tally.Count == 0)
        {
            Debug.Log("[VisionConeDebug] Nothing is cutting the cone right now.");
            return;
        }

        var lines = new List<string>();
        foreach (var pair in tally)
        {
            Collider c = pair.Key;
            Deaths d = pair.Value;
            Bounds b = c.bounds;
            lines.Add($"{d.Count,4} rayos | capa {LayerMask.LayerToName(c.gameObject.layer)} " +
                      $"| mueren a {d.MinDistance:F2}..{d.MaxDistance:F2}m, a altura {d.MinHeight:F2}..{d.MaxHeight:F2} " +
                      $"| el collider llega a {b.max.y:F2} " +
                      $"| {Path(c.transform)}{(c.isTrigger ? "  [TRIGGER]" : "")}");
        }
        lines.Sort((a, b) => b.CompareTo(a));

        // The decisive experiment: re-cast the same fan from higher up. If the blocked count drops
        // sharply, the rays were dying on low geometry (tables, counters, shelves) that lives inside
        // the same mesh as the walls, and raising the eye is the fix. If it barely moves, they were
        // dying on real full-height walls and the cone is simply drawing the room correctly.
        var sweep = new List<string>();
        foreach (float lift in new[] { 0.0f, 0.4f, 0.8f, 1.2f, 1.6f })
        {
            var probe = new Dictionary<Collider, Deaths>();
            int blocked = Sample(probe, lift);
            sweep.Add($"    +{lift:F1}m (origen y={transform.position.y + lift:F2}) -> {blocked,3}/{_visual.resolution} rayos bloqueados");
        }

        Debug.Log($"[VisionConeDebug] origen actual y={transform.position.y:F2} | " +
                  $"{tally.Count} colliders estan cortando el cono:\n" +
                  string.Join("\n", lines) +
                  "\n\n  Subiendo el origen del rayo:\n" + string.Join("\n", sweep) +
                  "\n\n  Si el conteo baja fuerte al subir, los rayos morian en MOBILIARIO dentro de la malla " +
                  "de pared -> el arreglo es levantar el ojo, no cambiar capas. Si casi no se mueve, son " +
                  "paredes de verdad y el cono esta dibujando bien.");
    }

    // Mirrors PlayerVisionConeVisual.DrawCone exactly: same origin, same mask, same trigger rule.
    // Returns how many rays were blocked. `lift` raises the origin to test a different eye height.
    private int Sample(Dictionary<Collider, Deaths> tally, float lift)
    {
        if (lift == 0.0f) _blockedAt.Clear();

        int rays = Mathf.Max(2, _visual.resolution);
        float angle = _cone.visionAngle * Mathf.Deg2Rad;
        float half = angle * 0.5f;
        float increment = angle / (rays - 1);
        LayerMask mask = _cone.ResolvedVisualMask;
        Vector3 origin = transform.position + Vector3.up * lift;
        int blocked = 0;

        for (int i = 0; i < rays; i++)
        {
            float a = -half + increment * i;
            Vector3 dir = (transform.forward * Mathf.Cos(a)) + (transform.right * Mathf.Sin(a));

            if (!Physics.Raycast(origin, dir, out RaycastHit hit,
                                 _cone.visionRadius, mask, QueryTriggerInteraction.Ignore))
                continue;

            blocked++;
            if (lift == 0.0f) _blockedAt.Add(hit.point);
            if (tally == null) continue;
            tally.TryGetValue(hit.collider, out Deaths d);
            d.Add(hit.distance, hit.point.y);
            tally[hit.collider] = d;
        }

        return blocked;
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
