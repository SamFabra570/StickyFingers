using System.Collections.Generic;
using UnityEngine;

public class SettingsValues : MonoBehaviour
{
    public GraphicsSettings.SettingsType settingsType;
    
    public string[] values;

    public int savedIndex;
    
    private void Awake()
    {
        if (settingsType == GraphicsSettings.SettingsType.Resolution)
        {
            PopulateResolutions();
            
            for (int i = 0; i < GraphicsSettings.Instance.resolutionValues.Length; i++)
            {
                if (GraphicsSettings.Instance.resolutionValues[i].width == Screen.width &&
                    GraphicsSettings.Instance.resolutionValues[i].height == Screen.height)
                {
                    savedIndex = i;
                    break;
                }
            }
        }
    }

    private void PopulateResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;

        List<Resolution> filteredResolutions = new List<Resolution>();
        List<string> displayValues = new List<string>();

        foreach (Resolution resolution in resolutions)
        {
            // Ignore resolutions below 1280 pixels wide
            if (resolution.width < 1280)
                continue;

            // Check if we've already added this width/height
            bool duplicate = false;

            foreach (Resolution existing in filteredResolutions)
            {
                if (existing.width == resolution.width &&
                    existing.height == resolution.height)
                {
                    duplicate = true;
                    break;
                }
            }

            if (duplicate)
                continue;

            filteredResolutions.Add(resolution);
            displayValues.Add($"{resolution.width}x{resolution.height}");
        }

        GraphicsSettings.Instance.resolutionValues = filteredResolutions.ToArray();
        values = displayValues.ToArray();
    }
}
