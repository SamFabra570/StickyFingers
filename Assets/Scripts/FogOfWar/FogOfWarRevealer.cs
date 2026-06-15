using UnityEngine;

// Builds the white "reveal" geometry the fog-of-war reveal camera renders into the mask.
// Two shapes, both flat on the XZ plane (so a straight-down reveal camera sees them face-on):
//   1. A proxy that SHARES the player's vision-cone mesh (already wall-occluded — no drift).
//   2. A small always-visible circle around the player, so you are never blind right next to yourself.
// Both go on a dedicated reveal layer and use an unlit white material; the main camera must cull
// that layer so these proxies are invisible to the player.
public class FogOfWarRevealer : MonoBehaviour
{
    [Tooltip("Vision cone whose occluded mesh we mirror into the reveal mask. Auto-found in children if empty.")]
    [SerializeField] private PlayerVisionConeVisual visionConeVisual;

    [Tooltip("Unlit, fully white material rendered into the reveal mask (white = visible).")]
    [SerializeField] private Material revealMaterial;

    [Tooltip("Layer the reveal camera renders exclusively. Must exist in Tags & Layers.")]
    [SerializeField] private string revealLayerName = "VisionReveal";

    [Tooltip("Radius of the always-visible circle around the player.")]
    [SerializeField] private float playerRevealRadius = 2f;
    [SerializeField] private int circleSegments = 32;

    private void Start()
    {
        if (visionConeVisual == null)
            visionConeVisual = GetComponentInChildren<PlayerVisionConeVisual>();

        int layer = LayerMask.NameToLayer(revealLayerName);
        if (layer < 0)
        {
            Debug.LogWarning($"FogOfWarRevealer: layer '{revealLayerName}' does not exist. Create it in Tags & Layers.");
            return;
        }

        // Cone proxy — parented under the cone visual so it inherits the exact same transform, then
        // shares the cone's Mesh instance (reused every frame, so it stays in sync with no extra work).
        if (visionConeVisual != null && visionConeVisual.ConeMesh != null)
        {
            GameObject coneProxy = CreateProxy("FogReveal_Cone", visionConeVisual.transform, layer);
            coneProxy.GetComponent<MeshFilter>().sharedMesh = visionConeVisual.ConeMesh;
        }

        // Always-visible circle around the player, centred on the cone origin (the player's vision point).
        Transform circleParent = visionConeVisual != null ? visionConeVisual.transform : transform;
        GameObject circleProxy = CreateProxy("FogReveal_PlayerRadius", circleParent, layer);
        circleProxy.GetComponent<MeshFilter>().sharedMesh = BuildCircleMesh(playerRevealRadius, circleSegments);
    }

    private GameObject CreateProxy(string proxyName, Transform parent, int layer)
    {
        GameObject go = new GameObject(proxyName);
        go.transform.SetParent(parent, false); // keep identity local transform → aligned with parent
        go.layer = layer;

        go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = revealMaterial;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        return go;
    }

    // Flat triangle-fan disc on the local XZ plane.
    private Mesh BuildCircleMesh(float radius, int segments)
    {
        segments = Mathf.Max(3, segments);
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;
        float step = Mathf.PI * 2f / segments;
        for (int i = 0; i < segments; i++)
        {
            float a = step * i;
            vertices[i + 1] = new Vector3(Mathf.Sin(a), 0f, Mathf.Cos(a)) * radius;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3]     = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % segments + 1;
        }

        Mesh mesh = new Mesh { name = "FogRevealCircle" };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
