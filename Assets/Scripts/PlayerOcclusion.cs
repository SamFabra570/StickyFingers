using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOcclusion : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    private Transform player;

    [Header("Fade Settings")]
    public string fadeMaterialName = "fat tree";
    [Range(0f, 1f)]
    public float fadeAlpha = 0.25f;
    public float fadeSpeed = 5f;

    // Keeps track of the material instances we're currently fading
    private Dictionary<Renderer, Material[]> originalMaterials = new();
    private Dictionary<Renderer, Material[]> fadeMaterials = new();

    private void Start()
    {
        player = PlayerController.Instance.transform;
    }

    private void Update()
    {
        if (playerCamera == null)
            return;

        if (player == null)
        {
            player = PlayerController.Instance.transform;
        }

        // Keep track of what is currently being hit this frame
        HashSet<Renderer> currentlyBlocked = new HashSet<Renderer>();

        Vector3 direction = player.position - playerCamera.transform.position;
        float distance = direction.magnitude;

        Ray ray = new Ray(playerCamera.transform.position, direction.normalized);

        // RaycastAll because there could potentially be multiple objects
        // between the camera and player.
        RaycastHit[] hits = Physics.RaycastAll(ray, distance);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer == null)
                continue;

            Material[] materials = renderer.materials;

            bool hasFatTree = false;

            foreach (Material material in materials)
            {
                if (material != null && material.name.ToLower().StartsWith(fadeMaterialName.ToLower()))
                {
                    hasFatTree = true;
                    break;
                }
            }

            if (!hasFatTree)
                continue;

            currentlyBlocked.Add(renderer);

            FadeRenderer(renderer);
        }

        // Restore anything that was being faded but isn't blocking anymore
        List<Renderer> renderersToRestore = new List<Renderer>();

        foreach (Renderer renderer in fadeMaterials.Keys)
        {
            if (!currentlyBlocked.Contains(renderer))
            {
                renderersToRestore.Add(renderer);
            }
        }

        foreach (Renderer renderer in renderersToRestore)
        {
            RestoreRenderer(renderer);
        }
    }

    private void FadeRenderer(Renderer renderer)
    {
        // If we haven't made runtime material instances for this renderer yet,
        // do that now.
        if (!fadeMaterials.ContainsKey(renderer))
        {
            Material[] original = renderer.sharedMaterials;
            Material[] instances = renderer.materials;

            originalMaterials[renderer] = original;
            fadeMaterials[renderer] = instances;

            // Configure only the Fat Tree material(s)
            for (int i = 0; i < instances.Length; i++)
            {
                Material material = instances[i];

                if (material == null)
                    continue;

                if (!material.name.ToLower().StartsWith(fadeMaterialName.ToLower()))
                    continue;

                SetTransparent(material);
            }
        }

        // Fade the Fat Tree material(s)
        Material[] fadeMaterialArray = fadeMaterials[renderer];

        foreach (Material material in fadeMaterialArray)
        {
            if (material == null)
                continue;

            if (!material.name.ToLower().StartsWith(fadeMaterialName.ToLower()))
                continue;

            Color color = material.GetColor("_BaseColor");

            float targetAlpha = fadeAlpha;

            color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);

            material.SetColor("_BaseColor", color);
        }
    }

    private void RestoreRenderer(Renderer renderer)
    {
        if (!fadeMaterials.ContainsKey(renderer))
            return;

        Material[] materials = fadeMaterials[renderer];

        bool fullyVisible = true;

        foreach (Material material in materials)
        {
            if (material == null)
                continue;

            if (!material.name.ToLower().StartsWith(fadeMaterialName.ToLower()))
                continue;

            Color color = material.GetColor("_BaseColor");

            color.a = Mathf.MoveTowards(color.a, 1f, fadeSpeed * Time.deltaTime);

            material.SetColor("_BaseColor", color);

            if (color.a < 0.99f)
            {
                fullyVisible = false;
            }
        }

        // Once the fade has completely finished, switch back to opaque.
        if (fullyVisible)
        {
            foreach (Material material in materials)
            {
                if (material == null)
                    continue;

                if (!material.name.ToLower().StartsWith(fadeMaterialName.ToLower()))
                    continue;

                SetOpaque(material);

                Color color = material.GetColor("_BaseColor");
                color.a = 1f;
                material.SetColor("_BaseColor", color);
            }

            // We don't need to replace the material array.
            // The runtime material is now fully restored.
            fadeMaterials.Remove(renderer);
            originalMaterials.Remove(renderer);
        }
    }

    private void SetTransparent(Material material)
    {
        // URP Lit Surface Type
        material.SetFloat("_Surface", 1f);

        // Enable transparency blending
        material.SetFloat("_Blend", 0f);

        // Disable alpha clipping
        material.SetFloat("_AlphaClip", 0f);

        // Standard URP transparent settings
        material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetFloat("_ZWrite", 0f);

        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
    
    private void SetOpaque(Material material)
    {
        // URP Lit Surface Type
        material.SetFloat("_Surface", 0f);

        // Disable alpha clipping
        material.SetFloat("_AlphaClip", 0f);

        // Opaque blending
        material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
        material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
        material.SetFloat("_ZWrite", 1f);

        material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
    }
}