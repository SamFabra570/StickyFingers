using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ObjectData> sellables = new();
    public List<ObjectData> collectibles = new();

    public void AddItem(ObjectData item)
    {
        if (item.type == ObjectData.ObjectType.Collectible)
        {
            collectibles.Add(item);
        }
        else
        {
            sellables.Add(item);
        }
    }
}
