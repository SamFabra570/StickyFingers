using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Canvas mainMenu;
    public GameObject settingsMenu;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("HUB");
    }

    public void Settings()
    {
        mainMenu.enabled = false;
        settingsMenu.SetActive(true);
    }

    public void BackToMainMenu()
    {
        mainMenu.enabled = true;
        settingsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        //Application.Quit();
        EditorApplication.isPlaying = false;
    }
}

