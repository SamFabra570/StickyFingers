using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    public ObjectData data;

    public void Collect(PlayerInventory inventory)
    {
        inventory.AddItem(data);
        Destroy(gameObject);
    }
}
