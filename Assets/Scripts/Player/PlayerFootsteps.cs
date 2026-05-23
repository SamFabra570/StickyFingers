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
    [Tooltip("Footstep clips — one is picked at random each step for variety.")]
    public AudioClip[] footstepClips;
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
            PlayStep();
            // Faster movement → shorter gap between steps
            float t = Mathf.InverseLerp(walkSpeed, sprintSpeed, speed);
            _stepTimer = Mathf.Lerp(walkInterval, sprintInterval, t);
        }
    }

    private void PlayStep()
    {
        if (source == null || footstepClips == null || footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        source.PlayOneShot(clip, volume);
    }
}
