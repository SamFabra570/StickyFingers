using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PostGameManager : MonoBehaviour
{
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

        UpdateRunEfficiencyText();
    }

    private void UpdateRunEfficiencyText()
    {
        if (timeRemaining <= 10)
        {
            runEfficiencyText.text = ("Barely escaped!!");
        }
        else if (timeRemaining >= 30)
        {
            runEfficiencyText.text = ("Escaped with time to spare!");
        }
        
        runEfficiencyText.gameObject.SetActive(true);
    }

    public void BackToHUB()
    {
        SceneManager.LoadScene("HUB");
    }
}
