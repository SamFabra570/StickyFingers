using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Blends a dedicated "you've been spotted" post-processing Volume in and out
/// based on whether any enemy currently sees the player
/// (<see cref="PlayerController.IsDetected"/>).
///
/// Drives the Volume's WEIGHT (0→1) instead of editing override values, so it
/// never dirties the shared Volume Profile asset. Put whatever look you want in
/// that Volume (red Vignette, desaturation, chromatic aberration…).
/// </summary>
public class DetectionVignette : MonoBehaviour
{
    [Tooltip("Dedicated Volume holding the 'detected' look. Auto-found on this object if empty. Set its weight to 0 in the inspector.")]
    public Volume detectionVolume;

    [Range(0f, 1f)] public float maxWeight = 1f;
    [Tooltip("How fast the effect fades in/out.")]
    public float fadeSpeed = 4f;

    [Header("Pulse")]
    public bool pulse = true;
    public float pulseSpeed = 6f;
    [Range(0f, 0.5f)] public float pulseAmount = 0.15f;

    private float _weight;

    private void Awake()
    {
        if (detectionVolume == null) detectionVolume = GetComponent<Volume>();
        if (detectionVolume != null) detectionVolume.weight = 0f;
    }

    private void Update()
    {
        if (detectionVolume == null) return;

        bool detected = PlayerController.Instance != null && PlayerController.Instance.IsDetected;
        float target = detected ? maxWeight : 0f;
        _weight = Mathf.MoveTowards(_weight, target, fadeSpeed * Time.deltaTime);

        float w = _weight;
        if (detected && pulse)
            w += Mathf.Sin(Time.time * pulseSpeed) * pulseAmount * (_weight / Mathf.Max(0.0001f, maxWeight));

        detectionVolume.weight = Mathf.Clamp01(w);
    }
}
