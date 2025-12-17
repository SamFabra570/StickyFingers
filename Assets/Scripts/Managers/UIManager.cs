using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject pauseScreen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pauseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPauseScreen()
    {
        pauseScreen.SetActive(true);
        GameManager.Instance.PauseGame(1);
    }

    public void HidePauseScreen()
    {
        pauseScreen.SetActive(false);
        GameManager.Instance.PauseGame(2);
    }
}
