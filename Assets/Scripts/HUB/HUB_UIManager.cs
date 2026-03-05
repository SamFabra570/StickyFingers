using System;
using UnityEngine;
using UnityEngine.UI;

public class HUB_UIManager : MonoBehaviour
{
    public static HUB_UIManager Instance;

    public Canvas planningUI;
    public GameObject loadoutScreen;
    public GameObject detailsScreen;
    public Animator detailsScreenAnim;

    public Slider debtPaidFill;
    [SerializeField] private float totalDebt;
    public float debtPaid;

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
                CalculateDebtRemaining();
                break;
            case "Details":
                detailsScreenAnim.Play("ShowDetails");
                break;
            case "Loadout":
                detailsScreenAnim.Play("HideDetails");
                break;
        }
    }

    private void CalculateDebtRemaining()
    {   
        float debtRemaining = totalDebt - debtPaid;
        float normalizedDebt = debtRemaining / totalDebt;
        debtPaidFill.value = 1 - normalizedDebt;
    }
}
