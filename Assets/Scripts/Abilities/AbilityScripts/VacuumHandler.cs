using System;
using UnityEngine;

public class VacuumHandler : MonoBehaviour
{
    private GameObject enemyInRange;
    
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.activeSelf)
        {
            if (other.CompareTag("Enemy"))
            {
                enemyInRange = other.gameObject;
                enemyInRange.SetActive(false);

                enemyInRange = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) 
            enemyInRange = null;
    }
}
