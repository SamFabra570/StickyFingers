using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour, IUIMenu
{
    [SerializeField] private GameObject settingsMenuScreen;

    private SettingsValues currentSetting;
    private bool hasMatchedSavedValue;

    public GameObject firstSelected;
    
    public TextMeshProUGUI appliedSettingsPopup;
    
    private TextMeshProUGUI selectedText;
    
    public EventSystem eventSystem;

    private int index;

    public void OnShowMenu()
    {
        settingsMenuScreen.SetActive(true);
        //settingsMenuCanvas.enabled = true;
        
        eventSystem.SetSelectedGameObject(firstSelected);
        
        selectedText = eventSystem.currentSelectedGameObject.GetComponent<TextMeshProUGUI>();
        currentSetting = eventSystem.currentSelectedGameObject.GetComponent<SettingsValues>();
        
        Debug.Log(eventSystem.currentSelectedGameObject);
        
        UpdateSettingsUI(currentSetting.values);
    }

    public void OnHideMenu()
    {
        selectedText = null;
        currentSetting = null;
        
        hasMatchedSavedValue = false;
        
        settingsMenuScreen.SetActive(false);
        //settingsMenuCanvas.enabled = false;
    }

    private void UpdateSettingsUI(string[] values)
    {
        if (!hasMatchedSavedValue)
        {
            index = currentSetting.savedIndex;
        
            selectedText.text = values[index]; //Set setting value to whatever was saved
            //Debug.Log("should have updated text");
            
            hasMatchedSavedValue = true;

            return;
        }
        
        selectedText.text = values[index];
        currentSetting.savedIndex = index;

        //Debug.Log("should have updated text: " + selectedText.text);
    }

    public void OnNavigate(Vector2 input)
    {
        //Set selected element if its currently null
        if (eventSystem.currentSelectedGameObject == null)
            eventSystem.SetSelectedGameObject(firstSelected);
        
        //If selectedText variable is not the current selected object, set it to the current selected object
        if (selectedText.gameObject != eventSystem.currentSelectedGameObject)
        {
            Debug.Log(eventSystem.currentSelectedGameObject);
            
            selectedText = eventSystem.currentSelectedGameObject.GetComponent<TextMeshProUGUI>();
            currentSetting = eventSystem.currentSelectedGameObject.GetComponent<SettingsValues>();

            Debug.Log("Selected object: " + selectedText);
            index = currentSetting.savedIndex;
        }

        //If not on a settings option, reset selectedText variable
        if (currentSetting == null)
        {
            selectedText = null;
            return;
        }
        
        if (input.x > 0.5f) //Right input
        {
            
            if (index < currentSetting.values.Length - 1) 
                index++;
        }
        else if (input.x < -0.5f) //Left input
        {
            
            if (index > 0) 
                index--;
        }
        
        UpdateSettingsUI(currentSetting.values);
    }
    
    private IEnumerator AppliedSettingsNotif()
    {
        appliedSettingsPopup.text = (currentSetting.settingsType + " setting saved!");
        
        appliedSettingsPopup.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);

        appliedSettingsPopup.gameObject.SetActive(false);
    }

    public void OnSubmit()
    {
        //Apply settings
        GraphicsSettings.Instance.ApplyOptionChange(currentSetting.settingsType, currentSetting.savedIndex);

        StartCoroutine(AppliedSettingsNotif());
    }

    public void OnButtonNorth()
    {
        //Reset to base settings
    }

    public void OnCancel()
    {
        UIManager.Instance.HideMenu();
    }
}