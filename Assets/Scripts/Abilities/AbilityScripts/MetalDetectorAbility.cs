using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Metal Detector")]
public class MetalDetectorAbility : Ability
{
    public override void Activate(GameObject user)
    {
        CompassArrow arrow = user.GetComponentInChildren<CompassArrow>(true);
        
        if (arrow == null)
            Debug.Log("No CompassArrow found");

        ItemController[] items = FindObjectsByType<ItemController>(FindObjectsSortMode.None);
        
        GameObject bestItem = null;
        float highestValue = -1f;

        foreach (ItemController item in items)
        {
            if (item.referenceItem.itemPrice > highestValue)
            {
                highestValue = item.referenceItem.itemPrice;
                bestItem = item.gameObject;
            }
        }

        if (bestItem != null)
        {
            Debug.Log(bestItem);
            arrow.SetActive(true);
            arrow.SetTarget(bestItem.transform);
        }
    }

    public override void Deactivate(GameObject user)
    {
        CompassArrow arrow = user.GetComponentInChildren<CompassArrow>(true);

        if (arrow != null)
            arrow.SetActive(false);
    }
    
    
}
