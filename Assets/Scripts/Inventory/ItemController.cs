using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] public InventoryItemData referenceItem;
    [SerializeField] public bool isBeingSeen;
    public void Pickup()
    {
        GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem.Add(referenceItem);
        Destroy(gameObject);
    }
    
    [SerializeField] private GameObject replacementPrefab;

    // public void Replace()
    // {
    //     // Spawn replacement
    //     GameObject newObj = Instantiate(
    //         referenceItem.prefab,
    //         transform.position,
    //         transform.rotation
    //     );
    //
    //     // Optional: keep scale
    //     newObj.transform.localScale = transform.localScale;
    //
    //     // Optional: same parent
    //     newObj.transform.SetParent(transform.parent);
    //
    //     // Delete old object
    //     Destroy(gameObject);
    // }

    public void Update()
    {
        
    }

}
