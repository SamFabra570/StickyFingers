using System;
using UnityEngine;

public class IcyFloor : MonoBehaviour
{
    public GameObject iceFloor;
    private bool isIcy;

    private void Start()
    {
        iceFloor.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isIcy)
            {
                Debug.Log("Shit on froze");
                PlayerController.Instance.ToggleIcyFloor(true);
                isIcy = true; 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isIcy)
            {
                Debug.Log("yo chain fake that shit is not frozen");
                PlayerController.Instance.ToggleIcyFloor(false);
                isIcy = false;
            }
        }
    }
}
