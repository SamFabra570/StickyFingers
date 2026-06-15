using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BaseScoutEnemy))]
public class ScoutDetectionIcon : MonoBehaviour
{
    [Tooltip("Shown while suspicion is building (player in sight, scout not yet committed). Opacity scales from 0 to 1 as suspicion fills.")]
    public GameObject questionIcon;

    [Tooltip("Shown once the scout has committed to attacking the player.")]
    public GameObject exclamationIcon;

    [Tooltip("Optional CanvasGroup on questionIcon (or a child) used to drive opacity. If null, falls back to looking for a CanvasGroup or Image on questionIcon.")]
    public CanvasGroup questionCanvasGroup;

    [Tooltip("Optional Image on questionIcon used to drive opacity when no CanvasGroup is assigned.")]
    public Image questionImage;

    private BaseScoutEnemy scout;

    private void Awake()
    {
        scout = GetComponent<BaseScoutEnemy>();

        if (questionCanvasGroup == null && questionIcon != null)
            questionCanvasGroup = questionIcon.GetComponentInChildren<CanvasGroup>();
        if (questionImage == null && questionIcon != null)
            questionImage = questionIcon.GetComponentInChildren<Image>();
    }

    private void Start()
    {
        SetIcon(questionIcon, false);
        SetIcon(exclamationIcon, false);
    }

    private void Update()
    {
        if (scout == null || scout.stateMachine == null)
            return;

        bool committed = scout.stateMachine._CurrentState is EnemyScoutAttackState;

        if (committed)
        {
            SetIcon(exclamationIcon, true);
            SetIcon(questionIcon, false);
            return;
        }

        if (scout.suspicion > 0f && scout.detectionWarmup > 0f)
        {
            SetIcon(exclamationIcon, false);
            SetIcon(questionIcon, true);
            SetQuestionAlpha(Mathf.Clamp01(scout.suspicion / scout.detectionWarmup));
            return;
        }

        SetIcon(questionIcon, false);
        SetIcon(exclamationIcon, false);
    }

    private void SetIcon(GameObject icon, bool active)
    {
        if (icon != null && icon.activeSelf != active)
            icon.SetActive(active);
    }

    private void SetQuestionAlpha(float alpha)
    {
        if (questionCanvasGroup != null)
        {
            questionCanvasGroup.alpha = alpha;
            return;
        }

        if (questionImage != null)
        {
            Color c = questionImage.color;
            c.a = alpha;
            questionImage.color = c;
        }
    }
}
