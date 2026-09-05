using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostGameManager : MonoBehaviour
{
    public InputActionReference submitAction;
    
    private float extractedBounty;
    private float timeRemaining;
    
    [Header ("Run Results")]
    [SerializeField] private Transform debt;
    //[SerializeField] private TextMeshProUGUI runEfficiencyText;
    [SerializeField] private TextMeshProUGUI extractedBountyText;
    //[SerializeField] private TextMeshProUGUI timeRemainingText;
    
    public ItemSlot[] itemSlots;

    public ItemSlot safetySlot;

    [SerializeField] private Image debtPaidFill;

    [Header("Victory Screen")] 
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private Transform debtPosVictory;
    
    [SerializeField] private TextMeshProUGUI victoryText;
    
    [Header("Defeat Screen")] 
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private GameObject defeatScreenSafetySlot;
    [SerializeField] private Transform debtPosDefeat;
    
    [SerializeField] private TextMeshProUGUI defeatText;

    [Header("Mission Info")] 
    [SerializeField] private GameObject missionRequirements;

    [SerializeField] private GameObject missionStatusCheckmark;
    
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI missionDescriptionText;
    [SerializeField] private TextMeshProUGUI progressText;
    
    [SerializeField] private Image missionItemIcon;

    private void Start()
    {
        //runEfficiencyText.gameObject.SetActive(false);
        
        extractedBounty = GameManager.Instance.extractedBounty;
        timeRemaining = GameManager.Instance.timeRemaining;
        
        extractedBountyText.text = ("+" +  extractedBounty);
        //timeRemainingText.text = ("" +  (int) timeRemaining);
         
        UpdateRunResultsScreen(GameManager.Instance.successfulRun);
        UpdateDebtInfo();
        UpdateRunEfficiencyText(GameManager.Instance.successfulRun);
        
        //UpdateMissionInfo();
        
        PlayerController.Instance.inputMap.UI.Enable();
        PlayerController.Instance.inputMap.Player.Disable();
    }

    private void UpdateRunResultsScreen(bool wasRunSuccessful)
    {
        if (debt.gameObject.activeSelf)
            debt.gameObject.SetActive(true);
        
        //Successful run
        if (wasRunSuccessful)
        {
            PopulateInventory();
            
            victoryScreen.SetActive(true);
            debt.position = debtPosVictory.position;
            
            UpdateMissionInfo();
        }
        //Failed run
        else
        {
            victoryScreen.SetActive(false);
            defeatScreen.SetActive(true);
            
            //Has Safety slot
            if (GameManager.Instance.PlayerPassives.Has(PassiveAbilities.SafetySlot))
            {
                safetySlot.image.sprite = GameManager.Instance.safetySlotItem.data.icon;
                safetySlot.stackValueText.text = ("+ " + extractedBounty);
                
                //If item is part of a mission requirement, add to mission progress
                MissionManager.Instance.AddProgress(GameManager.Instance.safetySlotItem.data, GameManager.Instance.safetySlotItem.stackSize);
                
                defeatScreenSafetySlot.gameObject.SetActive(true);
                debt.position = debtPosDefeat.position;
                
                UpdateMissionInfo();
            }
            //Does not have safety slot
            else
            {
                debt.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateRunEfficiencyText(bool hasExtracted)
    {
        string gameOverCause = GameManager.Instance.endRunState;
        
        if (hasExtracted)
        {
            if (timeRemaining <= 10)
            {
                victoryText.text = ("Barely escaped!!");
            }
            else if (timeRemaining >= 30)
            {
                victoryText.text = ("Escaped with time to spare!");
            }
        }
        else
        {
            if (gameOverCause == "Time") 
                defeatText.text = ("Ran out of time!!"); 
            else if (gameOverCause == "Mage")
                defeatText.text = ("Caught by the Mage!");
        }
        
        //runEfficiencyText.gameObject.SetActive(true);
    }

    private void UpdateMissionInfo()
    {
        if (MissionManager.Instance.activeMission == null)
        {
            if (missionRequirements.activeSelf) 
                missionRequirements.SetActive(false);
            return;
        }
        
        if (!missionRequirements.activeSelf) 
            missionRequirements.SetActive(true);
        
        missionNameText.text = MissionManager.Instance.activeMission.missionName;
        missionDescriptionText.text = MissionManager.Instance.activeMission.description;

        missionItemIcon.sprite = MissionManager.Instance.activeMission.targetItem.icon;
        progressText.text = (MissionManager.Instance.activeMission.currentAmount + " / " + MissionManager.Instance.activeMission.requiredAmount);
        
        if (MissionManager.Instance.IsComplete) 
            missionStatusCheckmark.SetActive(true);
        else
            missionStatusCheckmark.SetActive(false);
    }
    
    private void UpdateDebtInfo()
    {
        debtPaidFill.fillAmount = GameManager.Instance.GetDebtPaidPercent();
        //debtPaidText.text = ("" + (GameManager.Instance.maxDebt - GameManager.Instance.remainingDebt));
        //totalDebtText.text = ("" + GameManager.Instance.maxDebt);
    }
    
    private void PopulateInventory()
    {
        List<InventoryItem> inventory = GameManager.Instance.postGameInventory;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.Count && inventory[i].data != null)
            {
                InventoryItem item = inventory[i];

                itemSlots[i].item = item;
                itemSlots[i].isFull = true;

                itemSlots[i].stackValueText.SetText("+ " + (item.data.itemPrice * item.stackSize));
                itemSlots[i].stackValueText.enabled = true;

                itemSlots[i].image.sprite = item.data.icon;

                itemSlots[i].content.SetActive(true);
                itemSlots[i].gameObject.SetActive(true);
            }
            else
            {
                // No item for this slot, so hide the entire slot.
                itemSlots[i].content.SetActive(false);
            }
        }
    }
    
    private void BackToHUB()
    {
        PlayerController.Instance.inputMap.UI.Disable();
        PlayerController.Instance.inputMap.Player.Enable();
        
        SceneManager.LoadScene("HUB");
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        BackToHUB();
    }
    
    private void OnEnable()
    {
        submitAction.action.performed += OnSubmit;
        submitAction.action.Enable();
    }

    private void OnDisable()
    {
        submitAction.action.performed -= OnSubmit;
        submitAction.action.Disable();
    }
}
