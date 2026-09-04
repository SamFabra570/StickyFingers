using UnityEngine;
using UnityEngine.UI;

public class AbilityUnlockIcon : MonoBehaviour
{
    private ProgressionManager progressionManager;
    
    [SerializeField] private Ability ability;
    
    private Image icon;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressionManager = ProgressionManager.Instance;
        icon = GetComponent<Image>();
        
        CheckUnlockState();
    }

    private void CheckUnlockState()
    {
        if (progressionManager.IsUnlocked(ability))
        {
            icon.sprite = ability.unlockedIcon;
            icon.color = Color.white;
        }
        else
        {
            icon.sprite = ability.lockedIcon;
            
            if (progressionManager.CanUnlock(ability))
                icon.color = Color.white;
            else
                icon.color = Color.gray6;
            
            
        }
    }
}
