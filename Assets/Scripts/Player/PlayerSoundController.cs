using UnityEngine;

/// <summary>
/// One-shot SFX for player actions (stealing / dropping loot).
/// Lives on the player; called by PlayerController. Follows the project's
/// per-object AudioSource + PlayOneShot convention.
/// </summary>
public class PlayerSoundController : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("AudioSource for player action SFX. Auto-found on this object if left empty.")]
    public AudioSource source;

    [Header("Clips")]
    public AudioClip stealClip;
    public AudioClip dropClip;
    [Range(0f, 1f)] public float volume = 1f;

    private void Awake()
    {
        if (source == null) source = GetComponent<AudioSource>();
    }

    /// <summary>Called by PlayerController when an object is stolen.</summary>
    public void PlaySteal() => Play(stealClip);

    /// <summary>Ready for a future "drop loot" action (no player drop trigger yet).</summary>
    public void PlayDrop() => Play(dropClip);

    private void Play(AudioClip clip)
    {
        if (source != null && clip != null)
            source.PlayOneShot(clip, volume);
    }
}
