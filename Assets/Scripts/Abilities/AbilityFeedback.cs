using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class AbilityFeedback : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    //private AbilitySlot slot;
    
    [SerializeField] private float blinkSpeed = 0.2f;
    [SerializeField] private Renderer playerRend;
    [SerializeField] private Material baseColour;

    private Coroutine blinkRoutine;
    private AbilityState previousState;

    private void Start()
    {
        playerRend = PlayerController.Instance.rend;
        baseColour = PlayerController.Instance.basePlayerMat;

        AbilitySlot slot =  AbilityManager.Instance.GetAbility(slotIndex);
        previousState = slot.state;
        
        Debug.Log("ability feedback started");
    }

    // Update is called once per frame
    void Update()
    {
        AbilitySlot currentSlot = AbilityManager.Instance.GetAbility(slotIndex);

        if (currentSlot == null)
            return;
        
        Debug.Log("CurrentState: " + currentSlot.state);
        Debug.Log("PreviousState: " + previousState);

        if (currentSlot.state != previousState)
        {
            Debug.Log("Updated state: " + currentSlot.state);
            StateChange(currentSlot.state);
            previousState = currentSlot.state;
        }
    }

    public void GetAbilitySlot(int slot)
    {
        slotIndex = slot;
    }
    
    private void StateChange(AbilityState state)
    {
        Debug.Log("trying to activate based on state");
        switch (state)
        {
            case AbilityState.Active:
                StopBlink();
                break;

            case AbilityState.Ending:
                Debug.Log("Start blinking bruh");
                if (blinkRoutine == null)
                    blinkRoutine = StartCoroutine(Blink());
                break;

            case AbilityState.Cooldown:
                StopBlink();
                break;
            
            case AbilityState.Ready:
                StopBlink();
                break;
        }
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            playerRend.material = baseColour;
            yield return new WaitForSeconds(blinkSpeed);
            
            AbilitySlot slot = AbilityManager.Instance.GetAbility(slotIndex);
            playerRend.material = slot.ability.abilityColour;
            
            yield return new WaitForSeconds(blinkSpeed);
        }
        
    }

    private void StopBlink()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        
        blinkRoutine = null;
    }
}
