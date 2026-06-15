using UnityEngine;

// Feeds the fog-of-war plane shader the reveal mask and the world-space rectangle that mask covers.
// Put this on (or point it at) the orthographic, straight-down reveal camera that renders the
// "VisionReveal" layer into a RenderTexture. The fog shader maps each fragment's world XZ into the
// mask via these globals, so the camera can follow the player or stay fixed — the fog stays in sync.
[RequireComponent(typeof(Camera))]
public class FogOfWarController : MonoBehaviour
{
    [Tooltip("RenderTexture the reveal camera draws into (white = visible). Defaults to the camera's targetTexture.")]
    [SerializeField] private RenderTexture revealMask;

    private static readonly int FogRevealMaskId   = Shader.PropertyToID("_FogRevealMask");
    private static readonly int FogRevealBoundsId = Shader.PropertyToID("_FogRevealBounds");

    private Camera _revealCamera;

    private void Awake()
    {
        _revealCamera = GetComponent<Camera>();
        if (revealMask == null)
            revealMask = _revealCamera.targetTexture;
    }

    private void LateUpdate()
    {
        if (revealMask == null || !_revealCamera.orthographic)
            return;

        Shader.SetGlobalTexture(FogRevealMaskId, revealMask);

        // Orthographic top-down coverage: half-height = orthographicSize, half-width scales with aspect.
        float halfHeight = _revealCamera.orthographicSize;
        float halfWidth  = halfHeight * _revealCamera.aspect;
        Vector3 c = transform.position;

        // xy = world-space center on XZ, zw = half extents on XZ.
        Shader.SetGlobalVector(FogRevealBoundsId, new Vector4(c.x, c.z, halfWidth, halfHeight));
    }
}
