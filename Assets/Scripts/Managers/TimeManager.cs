using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    
    [Header ("Timer Length (Seconds)")]
    [SerializeField] private float countdownTime = 300f;
    [SerializeField] private float extraTime = 60f;
    private float finalTime;
    
    public float remainingTime;
    private bool lastMinute;
    
    private float lastMinuteTime = 60f;
    private float warningTime;
    
    [Header ("UI Refs")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerBackground;
    [SerializeField] private GameObject lastMinuteEffect;
    
    [SerializeField] private Color fullTimeColour = Color.white;
    [SerializeField] private Color warningColour = Color.yellowNice;
    [SerializeField] private Color lastMinuteColour = Color.darkRed;

    public PortalSpawner portalSpawner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var passive = GameManager.Instance.PlayerPassives;

        if (passive.Has(PassiveAbilities.ExtraTime))
        {
            finalTime = countdownTime + extraTime;
            Debug.Log("Extra time added");
        }
        else 
            finalTime = countdownTime;
        
        lastMinuteEffect.SetActive(false);
        
        warningTime = finalTime / 2;
        
        remainingTime = finalTime;
    }

    private void Update()
    {
        remainingTime -= Time.deltaTime;
        UpdateTimerText();
        UpdateTimerColour();
        
        float normalizedTimeRemaining = remainingTime / finalTime;
        timerBackground.fillAmount = normalizedTimeRemaining;

        if (!lastMinute && remainingTime <= 60)
        {
            lastMinute = true;
            LastMinute();
        }

        if (remainingTime <= 0)
        {
            timerText.text = ("0:00");
            GameManager.Instance.EndGame(false, "Time");
        }
            
    }

    private void UpdateTimerColour()
    {
        if (remainingTime > finalTime - 30)
            return;
        
        if (remainingTime > warningTime)
        {
            // White -> Yellow
            float t = Mathf.InverseLerp(finalTime, warningTime, remainingTime);
            timerBackground.color = Color.Lerp(fullTimeColour, warningColour, t);
        }
        else if (remainingTime > 60f)
        {
            // Yellow -> Red
            float t = Mathf.InverseLerp(warningTime, 60f, remainingTime);
            timerBackground.color = Color.Lerp(warningColour, lastMinuteColour, t);
        }
        else
        {
            // Last minute
            //float t = Mathf.InverseLerp(60f, 0f, remainingTime);
            timerBackground.color = lastMinuteColour;
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
