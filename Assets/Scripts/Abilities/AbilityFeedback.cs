using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class AbilityFeedback : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    private AbilitySlot slot;
    
    [SerializeField] private float blinkSpeed = 0.2f;
    [SerializeField] private Renderer playerRend;
    [SerializeField] private Material baseColour;

    private Coroutine blinkRoutine;
    private AbilityState previousState =  AbilityState.Ready;

    private void Start()
    {
        playerRend = PlayerController.Instance.rend;
        baseColour = PlayerController.Instance.basePlayerMat;
    }

    // Update is called once per frame
    void Update()
    {
        slot = AbilityManager.Instance.GetAbility(slotIndex);
        
        if (slot != null)
            return;

        if (slot.state != previousState)
        {
            Debug.Log("attempt to activate based on state");
            StateChange(slot.state);
            previousState = slot.state;
        }
    }
    
    private void StateChange(AbilityState state)
    {
        switch (state)
        {
            case AbilityState.Active:
                StopBlink();
                break;

            case AbilityState.Ending:
                Debug.Log("Start blinking bruh");
                blinkRoutine = StartCoroutine(Blink());
                break;

            case AbilityState.Cooldown:
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
