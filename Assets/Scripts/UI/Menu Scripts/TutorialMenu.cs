using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialMenu : MonoBehaviour, IUIMenu
{
    public static TutorialMenu Instance;

    [SerializeField] private List<TutorialSegment> tutorialSegments = new();
    private List<Transform> elementsToFocus = new();
    private int index;

    private bool isTutorialActive;
    
    [Header ("Tutorial UI")]
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;

    private Transform currentFocusedElement;
    private Transform focusedElementOGParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        tutorialUI.SetActive(false);
    }
    
    public void OnShowMenu()
    {
        tutorialUI.SetActive(true);
        ShowTutorialUI();
    }
    
    public void OnHideMenu()
    {
        if (currentFocusedElement != null)
            UnfocusTutorialSegment();
        
        tutorialUI.SetActive(false);
    }

    public bool HasCompletedTutorial(TutorialSegment tutorial)
    {
        return GameManager.Instance.completedTutorials.Contains(tutorial.id);
    }

    private void ShowTutorialUI()
    {
        if (tutorialSegments[index].title != "") 
            titleText.text = tutorialSegments[index].title;
        
        titleText.color = Color.darkRed;
        bodyText.text = tutorialSegments[index].body;
        
        if (tutorialSegments[index].needsFocusedElement) 
            FocusTutorialSegment(elementsToFocus[index]);
        
        Debug.Log("Tutorial Segment Count: " + tutorialSegments.Count + "Index: " + index);
    }

    private void CompleteTutorial()
    {
        // if (GameManager.Instance.completedTutorials.Contains(tutorialSegments[index].id))
        // {
        //     Debug.Log("No more tutorial segments");
        //
        //     tutorialSegments.Clear();
        //     elementsToFocus.Clear();
        //     index = 0;
        //
        //     UIManager.Instance.HideMenu();
        //     
        //     return;
        // }
        
        GameManager.Instance.completedTutorials.Add(tutorialSegments[index].id);
        
        if (tutorialSegments[index].needsFocusedElement) 
            UnfocusTutorialSegment();
        
        index++;

        if (index < tutorialSegments.Count)
        {
            Debug.Log("Next tutorial segment");
            ShowTutorialUI();
            return;
        }

        Debug.Log("No more tutorial segments");

        tutorialSegments.Clear();
        elementsToFocus.Clear();
        index = 0;
        
        isTutorialActive = false;

        UIManager.Instance.HideMenu();
        
        // index++;
        //
        // if (index <= tutorialSegments.Count - 1)
        // {
        //     Debug.Log("Next tutorial segment");
        //     ShowTutorialUI();
        // }
        // else
        // {
        //     Debug.Log("No more tutorial segments");
        //     tutorialSegments.Clear();
        //     elementsToFocus.Clear();
        //     index = 0;
        //     
        //     UIManager.Instance.HideMenu();
        // }
    }

    public void CacheTutorialContent(List<TutorialSegment> segments, List<Transform> elements)
    {
        if (isTutorialActive)
            return;

        isTutorialActive = true;
        
        for (int i = 0; i < segments.Count; i++)
        {
            tutorialSegments.Add(segments[i]);
            
            elementsToFocus.Add(elements[i]);
        }
    }

    private void FocusTutorialSegment(Transform segmentFocus)
    {
        currentFocusedElement = segmentFocus;
        focusedElementOGParent = currentFocusedElement.parent;
        
        currentFocusedElement.SetParent(tutorialUI.transform);
        currentFocusedElement.SetAsFirstSibling();
    }

    private void UnfocusTutorialSegment()
    {
        currentFocusedElement.SetParent(focusedElementOGParent);
        currentFocusedElement.SetAsFirstSibling();
        
        currentFocusedElement = null;
        focusedElementOGParent = null;
    }

    public void OnSubmit()
    {
        CompleteTutorial();
    }

    public void OnButtonNorth()
    {
        Debug.Log("Skipping tutorial");
        
        GameManager.Instance.completedTutorials.Add(tutorialSegments[index].id);
        
        tutorialSegments.Clear();
        elementsToFocus.Clear();
        index = 0;
        
        isTutorialActive = false;
            
        UIManager.Instance.HideMenu();
    }
}