using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class AbilityFeedback : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    AbilitySlot slot;
    
    [SerializeField] private float blinkSpeed = 0.2f;
    private Renderer playerRend;
    private Material baseColour;

    private Coroutine blinkRoutine;
    private AbilityState previousState;

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
                blinkRoutine = StartCoroutine(Blink());
                break;

            case AbilityState.Cooldown:
            case AbilityState.Ready:
                StopBlink();
                break;
        }
    }

    private IEnumerator Blink()
    {
        playerRend.material = baseColour;
        yield return new WaitForSeconds(blinkSpeed);
        playerRend.material = slot.ability.abilityColour;
    }

    private void StopBlink()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        
        blinkRoutine = null;
    }
}
