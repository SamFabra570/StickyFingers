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
    [SerializeField] Renderer portalBase;
    private Material portalBaseMat;
    [SerializeField] Material portalChargedMat;

    private void Start()
    {
        portalBaseMat = portalBase.material;

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
        if (charge >= 0.8)
        {
            if (state == PortalState.Charged)
                candleFlames[4].SetActive(true);
            if (state != PortalState.Charged)
                candleFlames[4].SetActive(false);
        }
        if (charge >= 0.8 && charge <= 1)
        {
            if (state == PortalState.Charging)
                candleFlames[3].SetActive(true);
            if ((charge > 0.79 && charge < 0.81) && state == PortalState.Idle)
                candleFlames[3].SetActive(false);
        }
        if (charge >= 0.6 && charge <= 0.8)
        {
            if (state == PortalState.Charging)
                candleFlames[2].SetActive(true);
            if ((charge > 0.59 && charge < 0.61) && state == PortalState.Idle)
                candleFlames[2].SetActive(false);
        }
        if (charge >= 0.4 && charge <= 0.6)
        {
            if (state == PortalState.Charging)
                candleFlames[1].SetActive(true);
            if ((charge > 0.39 && charge < 0.41) && state == PortalState.Idle)
                candleFlames[1].SetActive(false);
        }
        if (charge >= 0 && charge <= 0.4)
        {
            if (charge >= 0.2 && state == PortalState.Charging)
                candleFlames[0].SetActive(true);
            if ((charge > 0.19 && charge < 0.21) && state == PortalState.Idle)
                candleFlames[0].SetActive(false);
        }
        
        //portalAnim.Play("RaisePortal", 0, chargePercent);
        //portalAnim.speed = 0;
        
        if (state == PortalState.Charged)
            portalBase.material =  portalChargedMat;
        else if (state != PortalState.Charged && portalBase.material != portalBaseMat)
            portalBase.material =  portalBaseMat;
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
