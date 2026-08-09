using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IUIMenu
{
    [SerializeField] private GameObject mainMenuScreen;
    
    public GameObject menuStartButton;
    
    [SerializeField] private EventSystem eventSystem;
    
    public void OnShowMenu()
    {
        //UIManager.Instance.settingsMenu.settingsMenuCanvas.enabled = false;
        
        mainMenuScreen.SetActive(true);
        
        eventSystem.SetSelectedGameObject(menuStartButton);
    }

    public void OnHideMenu()
    {
        mainMenuScreen.SetActive(false);
    }
    
    public void LoadScene()
    {
        UIManager.Instance.HideMenu();
        
        SceneManager.LoadScene("HUB");
    }
    
    public void QuitGame()
    {
        Application.Quit();
        //EditorApplication.isPlaying = false;
    }

    public void OnCancel()
    {
        UIManager.Instance.HideMenu();
    }
}
