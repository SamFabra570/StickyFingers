using UnityEngine;

// Soft cover (Model C). A trigger volume placed over mid-height props (tables, low walls, crates).
// While the player overlaps it, the player is partially concealed: enemies detect at shorter range
// and the scout's suspicion builds slower. This does NOT block line of sight — that is a real wall
// (full cover). The collider on this GameObject must be set to "Is Trigger".
[RequireComponent(typeof(Collider))]
public class CoverZone : MonoBehaviour
{
    [Tooltip("How much this cover conceals the player, 0 = none, 1 = max. The player caps the total, so soft cover never fully hides.")]
    [Range(0f, 1f)] public float concealment = 0.5f;

    private void Reset()
    {
        // Cover zones must be triggers so they never physically block the player.
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        // Greener = more concealment, so designers can read cover strength at a glance.
        Gizmos.color = new Color(0f, concealment, 1f - concealment, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        if (col is BoxCollider box)
            Gizmos.DrawCube(box.center, box.size);
        else
            Gizmos.DrawSphere(Vector3.zero, 0.5f);
    }
}
