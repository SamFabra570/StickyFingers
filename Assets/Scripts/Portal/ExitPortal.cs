using System;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    public PortalState state =  PortalState.Idle;

    [SerializeField] private float chargeTime = 5;
    private float currentCharge;
    [SerializeField] private float chargePercent;

    public GameObject[] candleFlames;
    public Animator portalAnim;
    [SerializeField] SpriteRenderer portalBase;
    private Color portalBaseColour;
    [SerializeField] Material portalChargedMat;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chargeLoop;     // loops while the portal is charging
    [SerializeField] private AudioClip chargedReady;   // one-shot when it reaches full charge
    [SerializeField] private AudioClip activateClip;   // one-shot when the player uses it

    private void Start()
    {
        //portalBaseColour = portalBase.color;

        foreach (GameObject candle in candleFlames)
        {
            candle.SetActive(false);
        }
    }

    void Update()
    {
        if (state == PortalState.Idle)
        {
            //Decrease portal charge if idle and charge is not at 0
            if (currentCharge > 0)
                currentCharge -= Time.deltaTime;
        }
        
        if (state == PortalState.Charging)
        {
            //Charge portal
            if (currentCharge < chargeTime) 
                currentCharge += Time.deltaTime;
            
            //If portal charge is max, set as Charged
            if (currentCharge >= chargeTime)
            {
                state = PortalState.Charged;
                //PlayerController.Instance.portalCharged = true;
                StopChargeLoop();
                if (audioSource != null && chargedReady != null)
                    audioSource.PlayOneShot(chargedReady);
            }

        }
        
        chargePercent = currentCharge / chargeTime;
        AnimatePortal(chargePercent);
    }

    private void AnimatePortal(float charge)
    {
        
        portalAnim.Play("ChargePortal", 0, charge);
        portalAnim.speed = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            state = PortalState.Charging;
            StartChargeLoop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            state = PortalState.Idle;
            StopChargeLoop();
        }
    }

    private void StartChargeLoop()
    {
        if (audioSource == null || chargeLoop == null) return;
        if (audioSource.isPlaying && audioSource.clip == chargeLoop) return;

        audioSource.clip = chargeLoop;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopChargeLoop()
    {
        if (audioSource != null && audioSource.clip == chargeLoop && audioSource.isPlaying)
            audioSource.Stop();
    }

    /// <summary>Called by PlayerController when the player uses the charged portal.</summary>
    public void PlayActivate()
    {
        if (audioSource != null && activateClip != null)
            audioSource.PlayOneShot(activateClip);
    }
}
