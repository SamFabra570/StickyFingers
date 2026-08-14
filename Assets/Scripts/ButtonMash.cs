using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMash : MonoBehaviour
{
    private PlayerController player;
    
    public bool isMashing;

    [SerializeField] private int buttonPressNeeded = 20;
    [SerializeField] public float maxEventTime = 15;
    [SerializeField] public float timeRemaining = 15;

    private float buttonMashInterval;

    // Update is called once per frame
    void Update()
    {
        if (isMashing)
        {
            //Countdown stun time normally
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }

            //End mashing event
            if (timeRemaining <= 0)
            {
                timeRemaining = maxEventTime;
                ToggleMashingEvent(false);
                //UIManager.Instance.ToggleInteractText(false, "");
                UIManager.Instance.mashBar.gameObject.SetActive(false);
                UIManager.Instance.isMashing = false;
                Destroy(gameObject);
                
                //Debug.Log("mashing ended");
                
                player.buttonMashObj = null;
                
                if (player.objectToSteal != null)
                {
                    player.interactType = 0;
                    player.interactable = player.objectToSteal.gameObject;
                    player.objectToSteal.SetHighlighted(true);
                    
                    UIManager.Instance.ShowPreviewItem(player.objectToSteal.referenceItem);
                    UIManager.Instance.ToggleInteractText(true, player.objectToSteal.tag); 
                }
                else
                {
                    PlayerController.Instance.interactable = null;
                    UIManager.Instance.ToggleInteractText(false, ""); 
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isMashing)
            {
                player = other.GetComponent<PlayerController>();
                
                if (player.objectToSteal != null)
                {
                    player.objectToSteal.SetHighlighted(false);
                }
                
                Debug.Log("GOTCHA!! U BETTER START MASHING MF");
                player.interactType = 2;
                player.interactable = gameObject;

                player.buttonMashObj = GetComponent<ButtonMash>();
                
                UIManager.Instance.ToggleInteractText(true, "MashEvent");
            
                //Debug.Log("Register mash interactable");
                
                //Set number of mashes needed for min time
                buttonMashInterval = maxEventTime / buttonPressNeeded;
                timeRemaining = maxEventTime;
                
                ToggleMashingEvent(true);
                UIManager.Instance.mashBar.gameObject.SetActive(true);
                UIManager.Instance.SetTriggeredObject(gameObject);
            }
        }
        
    }

    private void ToggleMashingEvent(bool mashingState)
    {
        if (mashingState != isMashing)
        {
            isMashing = mashingState;
            player.isFrozen = mashingState;
        }
    }

    public void MashEvent()
    {
        timeRemaining -= buttonMashInterval;
    }
}
