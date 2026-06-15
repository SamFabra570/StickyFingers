using System.Collections.Generic;
using UnityEngine;

// Tracks which CoverZones the player is currently standing in and exposes a single Concealment value.
// Lives on the same GameObject as the player's CharacterController so it receives OnTriggerEnter/Exit
// (a CharacterController fires trigger callbacks without needing a Rigidbody).
//
// Concealment = the strongest overlapping zone (MAX, not additive, so it stays bounded), then clamped
// by maxConcealment so soft cover never reaches the feel of a hard wall.
public class PlayerCover : MonoBehaviour
{
    [Tooltip("Upper bound on concealment. Soft cover should never fully hide the player (that is a real wall).")]
    [Range(0f, 1f)] public float maxConcealment = 0.6f;

    private readonly HashSet<CoverZone> _activeZones = new HashSet<CoverZone>();

    // 0 = fully exposed, up to maxConcealment = best available soft cover.
    public float Concealment
    {
        get
        {
            float strongest = 0f;
            foreach (CoverZone zone in _activeZones)
            {
                if (zone == null) continue;          // destroyed/unloaded zones leave stale refs
                if (zone.concealment > strongest)
                    strongest = zone.concealment;
            }
            return Mathf.Min(strongest, maxConcealment);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CoverZone zone = other.GetComponent<CoverZone>();
        if (zone != null)
            _activeZones.Add(zone);
    }

    private void OnTriggerExit(Collider other)
    {
        CoverZone zone = other.GetComponent<CoverZone>();
        if (zone != null)
            _activeZones.Remove(zone);
    }
}
