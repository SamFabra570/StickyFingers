using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialMenu : MonoBehaviour, IUIMenu
{
    public static TutorialMenu Instance;
    
    private HashSet<string> completedTutorials;

    private List<TutorialSegment> tutorialSegments;
    private List<Transform> elementsToFocus;
    private int index;
    
    [Header ("Tutorial UI")]
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;

    private GameObject currentFocusedElement;
    private Transform focusedElementOGParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    public void OnShowMenu()
    {
        tutorialUI.SetActive(true);
        
    }
    
    public void OnHideMenu()
    {
        if (currentFocusedElement != null)
            UnfocusTutorialSegment();
        
        tutorialUI.SetActive(false);
    }

    public bool HasCompletedTutorial(TutorialSegment tutorial)
    {
        return (completedTutorials.Contains(tutorial.id));
    }

    public void ShowTutorialUI(List<TutorialSegment> tutorial)
    {
        if (tutorial[index].title != "") 
            titleText.text = tutorial[index].title;
        titleText.color = Color.darkRed;
        bodyText.text = tutorial[index].body;
    }

    private void CompleteTutorial(TutorialSegment tutorial)
    {
        if (completedTutorials.Contains(tutorial.id)) return;
        
        completedTutorials.Add(tutorial.id);
        
        if (index < tutorialSegments.Count -1) 
            index++;
        else
        {
            tutorialSegments = null;
            elementsToFocus = null;
            index = 0;
        }
    }

    public void CacheTutorialContent(List<TutorialSegment> segments, List<Transform> elements)
    {
        for (int i = 0; i < segments.Count; i++)
        {
            tutorialSegments.Add(segments[i]);
            
            
            elementsToFocus.Add(elements[i]);
        }
    }

    public void FocusTutorialSegment(GameObject segmentFocus)
    {
        currentFocusedElement = segmentFocus;
        focusedElementOGParent = currentFocusedElement.transform.parent;
        
        segmentFocus.transform.SetParent(tutorialUI.transform);
    }

    private void UnfocusTutorialSegment()
    {
        currentFocusedElement.transform.SetParent(focusedElementOGParent);
    }

    public void OnSubmit()
    {
        CompleteTutorial(tutorialSegments[index]);
    }
}