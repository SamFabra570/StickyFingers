using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Canvas mainMenu;
    public GameObject settingsMenu;

    public GameObject menuStartButton;
    public GameObject settingsBackButton;
    
    private EventSystem eventSystem;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eventSystem = EventSystem.current;
        settingsMenu.SetActive(false);
        
        eventSystem.SetSelectedGameObject(menuStartButton);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("HUB");
    }

    public void Settings()
    {
        mainMenu.enabled = false;
        settingsMenu.SetActive(true);
        
        eventSystem.SetSelectedGameObject(settingsBackButton);
    }

    public void BackToMainMenu()
    {
        mainMenu.enabled = true;
        settingsMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(menuStartButton);
    }

    public void QuitGame()
    {
        Application.Quit();
        //EditorApplication.isPlaying = false;
    }
}

