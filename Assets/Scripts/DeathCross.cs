using System;
using UnityEngine;

public class DeathCross : MonoBehaviour
{
    private bool rotatable = true;
    private int rotationY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    private void OnCollisionEnter(Collision other)
    {
        rotatable = false;
        Time.timeScale = 0;
        
        gameObject.SetActive(false);
    }

    void Rotate()
    {
        if (rotatable)
        {
            rotationY++;
            transform.localRotation = Quaternion.Euler(0, rotationY, 0);
        }
        
    }
}
