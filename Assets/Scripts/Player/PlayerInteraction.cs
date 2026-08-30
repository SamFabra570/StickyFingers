using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerController player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = PlayerController.Instance;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (player.interactable != null)
        {
            //Debug.Log("Already interacting");
            return;
        }
        
        //Add object outline
        if (other.CompareTag("Object"))
        {
            other.GetComponent<ItemController>().SetHighlighted(true);
            
            player.interactType = 0;
            player.interactable = other.gameObject;
            player.objectToSteal = other.GetComponent<ItemController>();
            
            UIManager.Instance.ShowPreviewItem(player.objectToSteal.referenceItem);
            UIManager.Instance.ToggleInteractText(true, other.tag);
        }
        else if (other.CompareTag("Interactable"))
        {
            player.interactType = 1;
            player.interactable = other.gameObject;

            if (other.TryGetComponent(out MoleHole hole))
            {
                hole.playerInRange = true;
                hole.playerTransform = player.transform;
                
                UIManager.Instance.ToggleInteractText(true, "Mole");
            }
            else if (other.GetComponent<Chest>() != null)
            {
                UIManager.Instance.ToggleInteractText(true, "Chest");
            }
            else
            {
                //Debug.Log("Interactable");
                UIManager.Instance.ToggleInteractText(true, other.tag);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player.interactable)
        {
            UIManager.Instance.ToggleInteractText(false, "");
            Debug.Log("Interact text off");
        }
        
        if (other.CompareTag("Object"))
        {
            other.GetComponent<ItemController>().SetHighlighted(false);
            
            UIManager.Instance.DisablePreview();
            player.objectToSteal = null;
            player.interactable = null;

        }
        else if (other.CompareTag("Interactable"))
        {
            if (other.TryGetComponent(out MoleHole hole))
            {
                hole.playerInRange = false;
                hole.playerTransform = null;
            }
            
            player.interactable = null;
        }
    }
}
