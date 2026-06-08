using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PostGameManager : MonoBehaviour
{
    public InputActionReference submitAction;
    
    private float extractedBounty;
    private float timeRemaining;
    
    [SerializeField] private TextMeshProUGUI runEfficiencyText;
    [SerializeField] private TextMeshProUGUI extractedBountyText;
    [SerializeField] private TextMeshProUGUI timeRemainingText;

    private void Start()
    {
        runEfficiencyText.gameObject.SetActive(false);
        
        extractedBounty = GameManager.Instance.extractedBounty;
        timeRemaining = GameManager.Instance.timeRemaining;
        
        extractedBountyText.text = ("" +  extractedBounty);
        timeRemainingText.text = ("" +  (int) timeRemaining);
        
        UpdateRunEfficiencyText(GameManager.Instance.successfulRun);
        
        PlayerController.Instance.inputMap.UI.Enable();
        PlayerController.Instance.inputMap.Player.Disable();
    }

    private void UpdateRunEfficiencyText(bool hasExtracted)
    {
        string gameOverCause = GameManager.Instance.endRunState;
        
        if (hasExtracted)
        {
            if (timeRemaining <= 10)
            {
                runEfficiencyText.text = ("Barely escaped!!");
            }
            else if (timeRemaining >= 30)
            {
                runEfficiencyText.text = ("Escaped with time to spare!");
            }
        }
        else
        {
            if (gameOverCause == "Time") 
                runEfficiencyText.text = ("Ran out of time!!"); 
            else if (gameOverCause == "Mage")
                runEfficiencyText.text = ("Caught by the Mage!");
        }
        
        runEfficiencyText.gameObject.SetActive(true);
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        BackToHUB();
    }

    private void BackToHUB()
    {
        PlayerController.Instance.inputMap.UI.Disable();
        PlayerController.Instance.inputMap.Player.Enable();
        
        SceneManager.LoadScene("HUB");
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
