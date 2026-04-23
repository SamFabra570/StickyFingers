using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMash : MonoBehaviour
{
    private bool isMashing;

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
                UIManager.Instance.ToggleInteractText(false, "");
                UIManager.Instance.mashBar.gameObject.SetActive(false);
                UIManager.Instance.isMashing = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isMashing)
            {
                Debug.Log("GOTCHA!! U BETTER START MASHING MF");
                
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
            PlayerController.Instance.isFrozen = mashingState;
        }
    }

    public void MashEvent()
    {
        timeRemaining -= buttonMashInterval;
    }
}
