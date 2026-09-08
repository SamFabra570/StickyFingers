using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Diagnostic for "why does the enemy cone go through that wall?".
///
/// Re-casts the exact fan VisionCone draws, for every enemy cone in the scene, and reports what each
/// ray actually hit — by collider name, layer and path — so the offending object can be found instead
/// of guessed at. Also reports the world height the rays leave from, which is the other way a cone
/// goes through a wall: a ray fired above a low wall is not blocked by it, collider or no collider.
///
/// Bootstraps itself, so there is nothing to wire in the editor: enter Play mode and press the key.
/// </summary>
public class EnemyVisionDebug : MonoBehaviour
{
    public static KeyCode ReportKey = KeyCode.F10;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        GameObject host = new GameObject("[EnemyVisionDebug]");
        host.AddComponent<EnemyVisionDebug>();
        DontDestroyOnLoad(host);
    }

    private void Update()
    {
        if (Input.GetKeyDown(ReportKey))
            Report();
    }

    private void Report()
    {
        // FindObjectsByType, not FindObjectsOfType: the latter is obsolete on Unity 6 and this project is 6000.2.
        VisionCone[] cones = FindObjectsByType<VisionCone>(FindObjectsSortMode.None);
        if (cones.Length == 0)
        {
            Debug.Log("[EnemyVisionDebug] No hay ningun VisionCone activo en la escena.");
            return;
        }

        StringBuilder report = new StringBuilder();
        report.AppendLine($"[EnemyVisionDebug] {cones.Length} conos de enemigo:");

        foreach (VisionCone cone in cones)
        {
            // VisionCone converts VisionAngle to radians in Awake, so read it as radians here.
            float angle = cone.VisionAngle;
            int rays = Mathf.Max(2, cone.VisionConeResolution);
            float half = angle * 0.5f;
            float increment = angle / (rays - 1);

            Transform t = cone.transform;
            var tally = new Dictionary<Collider, int>();
            int clear = 0;

            for (int i = 0; i < rays; i++)
            {
                float a = -half + increment * i;
                Vector3 dir = (t.forward * Mathf.Cos(a)) + (t.right * Mathf.Sin(a));

                if (Physics.Raycast(t.position, dir, out RaycastHit hit,
                                    cone.VisionRange, cone.VisionObstructingLayer))
                {
                    tally.TryGetValue(hit.collider, out int n);
                    tally[hit.collider] = n + 1;
                }
                else
                {
                    clear++;
                }
            }

            report.AppendLine();
            report.AppendLine($"  {Path(t)}");
            report.AppendLine($"    origen de los rayos: y = {t.position.y:F2}  |  rango {cone.VisionRange}  " +
                              $"|  mask {MaskNames(cone.VisionObstructingLayer)}");
            report.AppendLine($"    {clear}/{rays} rayos llegan al final SIN chocar nada");

            if (tally.Count == 0)
            {
                report.AppendLine("    NADA esta cortando este cono.");
            }
            else
            {
                foreach (KeyValuePair<Collider, int> pair in tally)
                {
                    Collider c = pair.Key;
                    Bounds b = c.bounds;
                    report.AppendLine($"    {pair.Value,4} rayos | capa {LayerMask.LayerToName(c.gameObject.layer)} " +
                                      $"| alto {b.min.y:F2}..{b.max.y:F2} | {Path(c.transform)}");
                }
            }

            // The other half of the story: Sight is what actually detects, and it uses its own mask.
            Sight sight = cone.GetComponentInParent<Sight>();
            if (sight != null)
                report.AppendLine($"    Sight.obstacles_layer_ = {MaskNames(sight.obstacles_layer_)}  " +
                                  $"(esto es lo que bloquea la DETECCION, no el dibujo)");
        }

        report.AppendLine();
        report.AppendLine("Si un cono tiene muchos rayos 'SIN chocar nada' mirando a una pared: o la pared no esta " +
                          "en la mask, o su collider termina por debajo de la altura del origen y el rayo le pasa por arriba " +
                          "(compara 'alto' con 'origen de los rayos').");

        Debug.Log(report.ToString());
    }

    private static string MaskNames(LayerMask mask)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < 32; i++)
        {
            if ((mask.value & (1 << i)) == 0) continue;
            string n = LayerMask.LayerToName(i);
            names.Add(string.IsNullOrEmpty(n) ? i.ToString() : n);
        }
        return names.Count == 0 ? "NADA (vacia!)" : string.Join("+", names);
    }

    private static string Path(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
