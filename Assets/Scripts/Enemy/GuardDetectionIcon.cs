using UnityEngine;

[RequireComponent(typeof(BaseEnemy))]
public class GuardDetectionIcon : MonoBehaviour
{
    [Tooltip("Shown while the guard is committed to the player (Pursuit or Attack state). Hidden in Patrol/Search/Stunned.")]
    public GameObject exclamationIcon;

    private BaseEnemy guard;

    private void Awake()
    {
        guard = GetComponent<BaseEnemy>();
    }

    private void Start()
    {
        SetIcon(false);
    }

    private void Update()
    {
        if (guard == null || guard.stateMachine == null)
            return;

        var current = guard.stateMachine._CurrentState;
        bool committed = current is EnemyPursuitState || current is EnemyAttackState;

        SetIcon(committed);
    }

    private void SetIcon(bool active)
    {
        if (exclamationIcon != null && exclamationIcon.activeSelf != active)
            exclamationIcon.SetActive(active);
    }
}
