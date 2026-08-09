using System;
using Unity.VisualScripting;
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
        
        GraphicsSettings.Instance.SetFPS(1);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("HUB");
    }

    public void Settings()
    {
        mainMenu.gameObject.SetActive(false);
        settingsMenu.SetActive(true);
        
        eventSystem.SetSelectedGameObject(settingsBackButton);
    }

    public void BackToMainMenu()
    {
        mainMenu.gameObject.SetActive(false);
        settingsMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(menuStartButton);
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

