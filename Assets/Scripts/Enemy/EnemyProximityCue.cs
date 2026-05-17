using UnityEngine;

/// <summary>
/// Audio + visual "you sense them before you see them" cue for an enemy.
/// The proximity sound rises as the player gets closer; a pulsing ground
/// ring appears only while the player cannot see this enemy.
/// </summary>
public class EnemyProximityCue : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Visibility component of this enemy. Auto-found if left empty.")]
    public DitherVisibility ditherVisibility;
    [Tooltip("Looping AudioSource for the proximity sound. Optional.")]
    public AudioSource cueAudio;

    [Header("Proximity")]
    [Tooltip("The player must be within this distance for any cue to show.")]
    public float cueRange = 12f;
    [Tooltip("Volume of the proximity sound when the player is right on top of the enemy.")]
    public float maxVolume = 1f;

    [Header("Ground Pulse")]
    [Tooltip("Optional — a default unlit material is built at runtime if left empty.")]
    public Material pulseMaterial;
    public Color pulseColor = Color.white;
    public float pulseMaxRadius = 1.5f;
    public float pulseDuration = 1.2f;
    public float pulseWidth = 0.08f;
    public int pulseSegments = 40;
    [Tooltip("Lift of the ring above the enemy's origin — avoids z-fighting with the floor.")]
    public float groundYOffset = 0.05f;

    private Transform _player;
    private LineRenderer _pulseLine;
    private float _pulseTimer;

    private void Awake()
    {
        if (ditherVisibility == null)
            ditherVisibility = GetComponentInChildren<DitherVisibility>();

        BuildPulseRing();
    }

    //Builds the ground ring entirely in code — no prefab or asset needed.
    private void BuildPulseRing()
    {
        var ringGO = new GameObject("ProximityPulse");
        ringGO.transform.SetParent(transform, false);

        _pulseLine = ringGO.AddComponent<LineRenderer>();
        _pulseLine.useWorldSpace = true;
        _pulseLine.loop = true;
        _pulseLine.positionCount = pulseSegments;
        _pulseLine.widthMultiplier = pulseWidth;
        _pulseLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _pulseLine.receiveShadows = false;
        _pulseLine.material = pulseMaterial != null
            ? pulseMaterial
            : new Material(Shader.Find("Sprites/Default"));
        _pulseLine.enabled = false;
    }

    private void Update()
    {
        // PlayerController.Instance may not exist yet on the first frames.
        if (_player == null)
        {
            if (PlayerController.Instance == null) return;
            _player = PlayerController.Instance.transform;
        }

        float distance = Vector3.Distance(transform.position, _player.position);
        float proximity = 1f - Mathf.Clamp01(distance / cueRange);   // 0 far → 1 on top

        UpdateAudio(proximity);
        UpdatePulse(proximity > 0f);
    }

    //Louder the closer the player gets; silent past cueRange.
    private void UpdateAudio(float proximity)
    {
        if (cueAudio == null) return;

        cueAudio.volume = proximity * maxVolume;

        if (proximity > 0f && !cueAudio.isPlaying)
            cueAudio.Play();
        else if (proximity <= 0f && cueAudio.isPlaying)
            cueAudio.Stop();
    }

    //Pulsing ground ring — only while the player is close AND can't see this enemy.
    private void UpdatePulse(bool playerInRange)
    {
        bool show = playerInRange && (ditherVisibility == null || !ditherVisibility.IsVisible);

        if (!show)
        {
            if (_pulseLine.enabled) _pulseLine.enabled = false;
            _pulseTimer = 0f;
            return;
        }

        _pulseLine.enabled = true;
        _pulseTimer += Time.deltaTime;

        float t = (_pulseTimer % pulseDuration) / pulseDuration;   // 0 → 1, loops
        float radius = t * pulseMaxRadius;

        Color c = pulseColor;
        c.a = 1f - t;                                              // fade as it expands
        _pulseLine.startColor = _pulseLine.endColor = c;

        Vector3 center = transform.position + Vector3.up * groundYOffset;
        for (int i = 0; i < pulseSegments; i++)
        {
            float angle = (i / (float)pulseSegments) * Mathf.PI * 2f;
            Vector3 point = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            _pulseLine.SetPosition(i, point);
        }
    }
}
