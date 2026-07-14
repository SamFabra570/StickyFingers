using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] public InventoryItemData referenceItem;

    [SerializeField] private MeshRenderer meshRenderer;
    
    private Material baseMat;
    [SerializeField] Material outlineMat;

    [SerializeField] public bool isBeingSeen;
    
    private void Start()
    {
        meshRenderer.GetComponent<MeshRenderer>();
        
        baseMat =  meshRenderer.material;
    }

    public void SetHighlighted(bool highlighted)
    {
        if (meshRenderer == null)
        {
            Debug.Log("MeshRenderer is null");
            return;
        }
        
        if (baseMat == null)
        {
            Debug.Log("outlineMat is null");
            return;
        }
            
        
        meshRenderer.material = highlighted ? outlineMat : baseMat;
    }
    
    public void Pickup()
    {
        GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem.Add(referenceItem);
        Destroy(gameObject);
    }

}
