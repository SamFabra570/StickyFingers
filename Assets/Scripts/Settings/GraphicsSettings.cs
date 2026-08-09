using System;
using UnityEngine;

public class GraphicsSettings : MonoBehaviour
{
    public static GraphicsSettings Instance;

    public bool hasAppliedCustomSettings;

    [Header("Set Values")] 
    private int FPS;
    
    //[SerializeField] private int targetFPS;
    private readonly int[] fpsValues = {30, 60, 120, -1};
    public Resolution[] resolutionValues;
    private bool isFullscreen = true;
    
    //[SerializeField] private string[] options;
    
    public enum SettingsType
    {
        FPS,
        Resolution,
        Fullscreen,
        VSync
    }

    private void Awake()
    {
        Instance = this;
        
        //resolutionValues = Screen.resolutions;
        //SetFPS(PlayerPrefs.GetInt("FPS", targetFPS));
        //SetFPS(fpsValues[1]);
    }

    public void ApplyOptionChange(SettingsType type, int index)
    {
        switch (type)
        {
            case SettingsType.FPS:
                SetFPS(index);
                break;

            case SettingsType.Resolution:
                SetResolution(index);
                break;

            case SettingsType.Fullscreen:
                SetFullscreen(index);
                break;
        }

        hasAppliedCustomSettings = true;
    }
    
    public void SetFPS(int option)
    {
        QualitySettings.vSyncCount = 0;   // Disable VSync
        Application.targetFrameRate = fpsValues[option];
        
        Debug.Log("Set FPS to " + Application.targetFrameRate);
    }

    private void SetResolution(int index)
    {
        Debug.Log(""+ index);
        
        Screen.SetResolution(resolutionValues[index].width, resolutionValues[index].height, isFullscreen);
        
        Debug.Log("Set resolution to " + Screen.currentResolution);
    }

    private void SetFullscreen(int index)
    {
        if (index == 0)
        {
            isFullscreen = true;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (index == 1)
        {
            isFullscreen = false;
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        
        Debug.Log("Set fullscreen to " + Screen.fullScreenMode);
    }
}
