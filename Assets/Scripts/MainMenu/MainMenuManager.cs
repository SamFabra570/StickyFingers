using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Canvas mainMenu;
    public GameObject settingsMenu;
    
    public InputActionReference cancelAction;

    public GameObject menuStartButton;
    public GameObject settingsBackButton;

    public GameObject helpScreen;
    
    public EventSystem eventSystem;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (settingsBackButton.activeSelf) 
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

    private void OnEnable()
    {
        cancelAction.action.performed += OnCancel;
        cancelAction.action.Enable();
    }
    
    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log("cancel");
        ToggleHelpScreen("Hide");
    }
    
    public void ToggleHelpScreen(string state)
    {
        if (state == "Show") 
            helpScreen.SetActive(true);
        else if (state == "Hide")
            helpScreen.SetActive(false);
    }
}

