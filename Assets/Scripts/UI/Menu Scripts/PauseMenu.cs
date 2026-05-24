using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour, IUIMenu
{
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private GameObject pauseMenuUI;
    
    [SerializeField] private GameObject firstItem;
    
    public void OnShowMenu()
    {
        GameManager.Instance.PauseGame(1);
        PlayerController.Instance.isPaused = true;
        
        pauseMenuUI.SetActive(true);
        
        if (firstItem != null)
            eventSystem.SetSelectedGameObject(firstItem);
    }

    public void OnHideMenu()
    {
        GameManager.Instance.PauseGame(0);
        PlayerController.Instance.isPaused = false;
        
        if (pauseMenuUI != null) 
            pauseMenuUI.SetActive(false);
    }

    public void OnCancel()
    {
        UIMenuStack.Pop();
    }
    
    public void OnSubmit() { }
}
