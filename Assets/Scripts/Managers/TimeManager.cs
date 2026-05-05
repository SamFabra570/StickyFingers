using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header ("Timer Length (Seconds)")]
    [SerializeField] private float countdownTime = 300f;
    [SerializeField] private float extraTime = 60f;
    
    private float remainingTime;
    private bool lastMinute;
    
    [Header ("UI Refs")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject lastMinuteEffect;

    public PortalSpawner portalSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var passive = GameManager.Instance.PlayerPassives;

        if (passive.Has(PassiveAbilities.ExtraTime))
        {
            remainingTime = countdownTime + extraTime;
            Debug.Log("Extra time added");
        }
        else 
            remainingTime = countdownTime;
        
        lastMinuteEffect.SetActive(false);
    }

    private void Update()
    {
        remainingTime -= Time.deltaTime;
        UpdateTimerText();

        if (!lastMinute && remainingTime <= 60)
        {
            lastMinute = true;
            LastMinute();
        }

        if (remainingTime <= 0)
        {
            timerText.text = ("0:00");
            GameManager.Instance.EndGame();
        }
            
    }

    private void LastMinute()
    {
        portalSpawner.SpawnPortal();
        lastMinuteEffect.SetActive(true);
    }

    private void UpdateTimerText()
    {
        timerText.text = FormatTime(remainingTime);
    }

    private string FormatTime(float time)
    {
        int mins = Mathf.FloorToInt(time / 60);
        int secs = Mathf.FloorToInt(time % 60);
        return string.Format("{0}:{1:00}", mins, secs);
    }
}
