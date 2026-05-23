using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Per-enemy audio. Two independent features you enable per enemy:
///  - Movement loop (e.g. the scout's wings): a looping AudioSource whose
///    volume tracks how fast the enemy is moving.
///  - Footsteps (e.g. the guard): one-shots paced by the NavMeshAgent speed.
/// Follows the project's per-object AudioSource convention. Reusable on all
/// three enemy types — just tick what each one needs.
/// </summary>
public class EnemyAudio : MonoBehaviour
{
    [Header("Movement Loop (e.g. scout wings)")]
    public bool useMovementLoop = false;
    [Tooltip("Looping AudioSource for the movement sound. Assign its clip; loop is forced on.")]
    public AudioSource loopSource;
    [Range(0f, 1f)] public float loopMinVolume = 0.1f;   // volume while standing still
    [Range(0f, 1f)] public float loopMaxVolume = 1f;     // volume at full speed

    [Header("Footsteps (e.g. guard)")]
    public bool useFootsteps = false;
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    [Range(0f, 1f)] public float footstepVolume = 1f;
    [Tooltip("Seconds between steps at the agent's top speed.")]
    public float footstepInterval = 0.5f;
    [Tooltip("Below this speed the enemy counts as standing still.")]
    public float moveThreshold = 0.1f;

    private NavMeshAgent _agent;
    private float _stepTimer;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (useMovementLoop && loopSource != null)
        {
            loopSource.loop = true;
            if (!loopSource.isPlaying) loopSource.Play();
        }
    }

    private void Update()
    {
        if (_agent == null) return;

        float speed = _agent.velocity.magnitude;
        float speedRatio = _agent.speed > 0.01f ? Mathf.Clamp01(speed / _agent.speed) : 0f;

        if (useMovementLoop) UpdateLoop(speedRatio);
        if (useFootsteps)    UpdateFootsteps(speed);
    }

    private void UpdateLoop(float speedRatio)
    {
        if (loopSource == null) return;
        loopSource.volume = Mathf.Lerp(loopMinVolume, loopMaxVolume, speedRatio);
    }

    private void UpdateFootsteps(float speed)
    {
        if (footstepSource == null || footstepClips == null || footstepClips.Length == 0) return;

        if (speed < moveThreshold)
        {
            _stepTimer = 0f;
            return;
        }

        _stepTimer -= Time.deltaTime;
        if (_stepTimer <= 0f)
        {
            footstepSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)], footstepVolume);
            _stepTimer = footstepInterval;
        }
    }
}
