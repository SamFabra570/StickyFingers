using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSegment", menuName = "TutorialSegment")]
public class TutorialSegment : ScriptableObject
{
    public string id;
    public string title;
    [TextArea(5, 10)]
    public string body;

    public bool needsFocusedElement;
}
