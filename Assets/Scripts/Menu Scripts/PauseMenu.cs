using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour, IUIMenu
{
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private GameObject pauseMenuUI;
    
    [SerializeField] private GameObject firstItem;
    
    public void OnShowMenu()
    {
        PlayerController.Instance.isPauseOpen = true;
        
        pauseMenuUI.SetActive(true);
        
        if (firstItem != null)
            eventSystem.SetSelectedGameObject(firstItem);
    }

    public void OnHideMenu()
    {
        PlayerController.Instance.isPauseOpen = false;
        
        if (pauseMenuUI != null) 
            pauseMenuUI.SetActive(false);
    }

    public void OnCancel()
    {
        UIManager.Instance.HideMenu();
    }
}
