using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame(int pauseState)
    {
        if (pauseState == 1)
        {
            Time.timeScale = 0;
        }

        if (pauseState == 2)
        {
            Time.timeScale = 1;
        }
        
    }
}
