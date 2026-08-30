using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BaseScoutEnemy))]
public class ScoutDetectionIcon : MonoBehaviour
{
    [Tooltip("Shown while suspicion is building (player in sight, scout not yet committed).")]
    public GameObject exclamationIcon;
    public Image exclamationIconFill;

    [Tooltip("Optional CanvasGroup used to drive opacity.")]
    public CanvasGroup questionCanvasGroup;

    private BaseScoutEnemy scout;

    private void Awake()
    {
        scout = GetComponent<BaseScoutEnemy>();
    }

    private void Start()
    {
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
            SetFill(1.0f);
            return;
        }

        //Reads the perception meter rather than the scout's old private suspicion counter, so the icon
        //now fills for ANY reason awareness is rising — including an ally shouting, which the player can
        //finally see happening instead of being ambushed by knowledge they had no way to observe.
        EnemyPerception perception = scout.perception;
        float progress = perception != null ? perception.Awareness01 : 0.0f;

        if (progress > 0.01f)
        {
            SetIcon(exclamationIcon, true);
            SetFill(Mathf.Clamp01(progress + 0.2f));
            return;
        }

        SetIcon(exclamationIcon, false);
        SetFill(0.0f);
    }

    private void SetFill(float amount)
    {
        if (exclamationIconFill != null)
            exclamationIconFill.fillAmount = amount;
    }

    private void SetIcon(GameObject icon, bool active)
    {
        if (icon != null && icon.activeSelf != active)
            icon.SetActive(active);
    }
}
