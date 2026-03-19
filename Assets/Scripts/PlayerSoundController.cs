using System;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Add object outline
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Un enemigo te ha detectacdo");
            
        }
    }
}
