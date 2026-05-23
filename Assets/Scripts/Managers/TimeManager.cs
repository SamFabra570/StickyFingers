using System;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    
    [Header ("Timer Length (Seconds)")]
    [SerializeField] private float countdownTime = 300f;
    [SerializeField] private float extraTime = 60f;
    
    public float remainingTime;
    private bool lastMinute;
    
    [Header ("UI Refs")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject lastMinuteEffect;

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
            GameManager.Instance.EndGame(false);
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
