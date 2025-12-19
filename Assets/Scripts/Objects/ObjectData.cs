using UnityEngine;

[CreateAssetMenu(fileName = "ObjectData", menuName = "Scriptable Objects/ObjectData")]
public class ObjectData : ScriptableObject
{
    public enum ObjectType
    {
        Sellable,
        Collectible
    }
    
    public string objectName;
    public GameObject worldModel;
    public int value;
    public ObjectType type;
}
