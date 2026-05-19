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
            state = PortalState.Charging;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
            state = PortalState.Idle;
    }
}
