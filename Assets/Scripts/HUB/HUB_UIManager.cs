using System;
using UnityEngine;

public class HUB_UIManager : MonoBehaviour
{
    public static HUB_UIManager Instance;

    public Canvas planningUI;
    public GameObject loadoutScreen;
    public GameObject detailsScreen;
    public Animator detailsScreenAnim;

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
        TogglePlanningUI("Close");
    }

    public void TogglePlanningUI(string status)
    {
        switch (status)
        {
            case "Close":
                planningUI.enabled = false;
                if (PlayerController.Instance.isFrozen)
                    PlayerController.Instance.isFrozen = false;
                break;
            case "Show":
                planningUI.enabled = true;
                //detailsScreen.SetActive(true);
                //loadoutScreen.SetActive(true);
                break;
            case "Details":
                detailsScreenAnim.Play("ShowDetails");
                //loadoutScreen.SetActive(false);
                //detailsScreen.SetActive(true);
                break;
            case "Loadout":
                detailsScreenAnim.Play("HideDetails");
                //detailsScreen.SetActive(false);
                //loadoutScreen.SetActive(true);
                break;
        }
    }
}
