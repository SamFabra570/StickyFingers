using UnityEngine;

/// <summary>
/// Plays footstep one-shots while the player moves on the ground.
/// Step cadence speeds up with movement speed, so sprinting sounds faster than
/// walking — mirroring the noise the player makes (see SoundPlayer radius).
/// Lives on the player root (where the CharacterController is). Follows the
/// project's per-object AudioSource + PlayOneShot convention.
/// </summary>
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("AudioSource used for footsteps. Auto-found on this object if left empty.")]
    public AudioSource source;
    [Tooltip("Walk footstep clips — one is picked at random each step for variety.")]
    public AudioClip[] footstepClips;
    [Tooltip("Run/sprint footstep clips — used when moving fast. Falls back to walk clips if empty.")]
    public AudioClip[] runClips;
    [Range(0f, 1f)]
    [Tooltip("Speed ratio (0=walk, 1=sprint) above which run clips are used instead of walk clips.")]
    public float runClipsThreshold = 0.5f;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Cadence")]
    [Tooltip("Seconds between steps at walking speed.")]
    public float walkInterval = 0.5f;
    [Tooltip("Seconds between steps at full sprint.")]
    public float sprintInterval = 0.3f;
    [Tooltip("Horizontal speed treated as 'walking' for cadence scaling.")]
    public float walkSpeed = 5f;
    [Tooltip("Horizontal speed treated as 'sprinting' for cadence scaling.")]
    public float sprintSpeed = 7f;
    [Tooltip("Below this horizontal speed the player counts as standing still.")]
    public float moveThreshold = 0.3f;

    private CharacterController _cc;
    private float _stepTimer;

    private void Awake()
    {
        if (source == null) source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // PlayerController adds the CharacterController at runtime — grab it lazily.
        if (_cc == null)
        {
            _cc = GetComponent<CharacterController>();
            if (_cc == null) return;
        }

        Vector3 v = _cc.velocity;
        v.y = 0f;
        float speed = v.magnitude;

        if (speed < moveThreshold || !_cc.isGrounded)
        {
            _stepTimer = 0f;   // reset so the next step lands the moment we move again
            return;
        }

        _stepTimer -= Time.deltaTime;
        if (_stepTimer <= 0f)
        {
            // Faster movement → run clips + shorter gap between steps
            float t = Mathf.InverseLerp(walkSpeed, sprintSpeed, speed);
            PlayStep(t);
            _stepTimer = Mathf.Lerp(walkInterval, sprintInterval, t);
        }
    }

    private void PlayStep(float speedRatio)
    {
        // Use run clips when moving fast; fall back to walk clips if run isn't assigned.
        AudioClip[] pool = (speedRatio >= runClipsThreshold && runClips != null && runClips.Length > 0)
            ? runClips
            : footstepClips;
        if (source == null || pool == null || pool.Length == 0) return;
        AudioClip clip = pool[Random.Range(0, pool.Length)];
        source.PlayOneShot(clip, volume);
    }
}
