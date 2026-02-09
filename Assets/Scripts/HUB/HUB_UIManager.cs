using System;
using UnityEngine;

public class HUB_UIManager : MonoBehaviour
{
    public static HUB_UIManager Instance;

    public Canvas planningUI;

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
        TogglePlanningUI(0);
    }

    public void TogglePlanningUI(int status)
    {
        switch (status)
        {
            case 0:
                planningUI.enabled = false;
                if (PlayerController.Instance.isFrozen)
                    PlayerController.Instance.isFrozen = false;
                break;
            case 1:
                planningUI.enabled = true;
                break;
        }
    }
}
