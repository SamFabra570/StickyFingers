using System;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    public PortalState state =  PortalState.Idle;

    [SerializeField] private float chargeTime = 5;
    private float currentCharge;
    [SerializeField] private float chargePercent;

    public Animator portalAnim;
    [SerializeField] Renderer portalBase;
    private Material portalBaseMat;
    [SerializeField] Material portalChargedMat;

    private void Start()
    {
        portalBaseMat = portalBase.material;
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
                PlayerController.Instance.portalCharged = true;
            }
                
        }
        
        chargePercent = currentCharge / chargeTime;
        AnimatePortal();
    }

    private void AnimatePortal()
    {
        portalAnim.Play("RaisePortal", 0, chargePercent);
        portalAnim.speed = 0;
        
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
