using System;
using System.Collections.Generic;
using UnityEngine;

public class HUBTutorialManager : MonoBehaviour
{
    public static HUBTutorialManager Instance;

    public GameObject loadoutMenuPrompt;
    public GameObject progressionMenuPrompt;
    public GameObject portalPrompt;
    
    [Header("Tutorial")] 
    public List<TutorialSegment> segments = new();
    //public List<Transform> elementsToFocus = new();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        progressionMenuPrompt.SetActive(false);
        portalPrompt.SetActive(false);
        loadoutMenuPrompt.SetActive(false);
    }

    private void Update()
    {
        if (TutorialMenu.Instance.HasCompletedTutorial(segments[0]))
        {
            progressionMenuPrompt.SetActive(true);
        }
        if (TutorialMenu.Instance.HasCompletedTutorial(ProgressionMenu.Instance.progressionMenuTutorial[0]))
        {
            loadoutMenuPrompt.SetActive(true);
        }
        if (TutorialMenu.Instance.HasCompletedTutorial(LoadoutMenu.Instance.segments[0]))
        {
            portalPrompt.SetActive(true);
        }
    }

    public void StartHUBTutorial()
    {
        if (!TutorialMenu.Instance.HasCompletedTutorial(segments[0]))
        {
            TutorialMenu.Instance.CacheTutorialContent(segments, null);
            
            UIManager.Instance.OpenMenu("TutorialMenu");
        }
    }
    
    
}
