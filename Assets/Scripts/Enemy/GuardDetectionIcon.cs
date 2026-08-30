using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BaseEnemy))]
public class GuardDetectionIcon : MonoBehaviour
{
    [Tooltip("Shown once the guard starts noticing the player. Fills as awareness builds, solid once committed.")]
    public GameObject exclamationIcon;

    [Tooltip("Optional radial Image driven by how close the guard is to committing. Leave empty for a plain on/off icon.")]
    public Image exclamationIconFill;

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

        //This used to be a hard on/off driven by the state: nothing at all, then suddenly a chase. The
        //player had no way to know they were being noticed until it was already too late to do anything
        //about it, which reads as unfair even when the AI is behaving correctly. Now the meter is visible
        //while it fills, so backing off is a real, informed choice.
        EnemyPerception perception = guard.perception;

        if (perception == null)
        {
            var current = guard.stateMachine._CurrentState;
            SetIcon(current is EnemyPursuitState || current is EnemyAttackState);
            return;
        }

        bool committed = perception.Level == EnemyPerception.Awareness.Alert;
        float progress = perception.Awareness01;

        if (committed)
        {
            SetIcon(true);
            SetFill(1.0f);
            return;
        }

        if (progress > 0.01f)
        {
            SetIcon(true);
            SetFill(Mathf.Clamp01(progress + 0.2f));
            return;
        }

        SetIcon(false);
        SetFill(0.0f);
    }

    private void SetFill(float amount)
    {
        if (exclamationIconFill != null)
            exclamationIconFill.fillAmount = amount;
    }

    private void SetIcon(bool active)
    {
        if (exclamationIcon != null && exclamationIcon.activeSelf != active)
            exclamationIcon.SetActive(active);
    }
}
