using UnityEngine;

[CreateAssetMenu(fileName = "ObjectData", menuName = "Scriptable Objects/ObjectData")]
public class ObjectData : ScriptableObject
{
    public enum ObjectType
    {
        Sellable,
        Collectible
    }

    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;
    public int value;
    public float weight;
    //public string description;
    public ObjectType type;
}
