using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float maxDebt;
    public float totalDebt;
    public float maxWeight;
    public bool runState;

    private void Awake()
    {
        if (Instance == null)
        { 
            //initialize the players inventory
            Instance = this;
        }
        else if (Instance != null )
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); 
        
    }
    public void PauseGame(int pauseState)
    {
        switch (pauseState)
        {
            case 0:
                Time.timeScale = 1;
                break;
            case 1:
                Time.timeScale = 0;
                break;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void EndGame()
    {
        AbilityManager.Instance.DeactivateAbilitiesGameOver();
        //Debug.Log("Game Over: YOU LOSE");
        GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem.SellInventory(runState);
        SceneManager.LoadScene("Post-Game");
        runState = false;
    }

    public void UpdateSpeed(float currentWeight)
    {
        if (currentWeight > maxWeight*0.5f && currentWeight < maxWeight*0.7f)
        {
            GameObject.Find("Player(Clone)").GetComponent<PlayerController>().baseMoveSpeed = 4.5f;
        }
        else if (currentWeight > maxWeight * 0.7f && currentWeight < maxWeight * 0.9f)
        {
            GameObject.Find("Player(Clone)").GetComponent<PlayerController>().baseMoveSpeed = 4f;
        }
        else if (currentWeight > maxWeight * 0.9f && currentWeight < maxWeight )
        {
            GameObject.Find("Player(Clone)").GetComponent<PlayerController>().baseMoveSpeed = 3f;
        }
        else if (currentWeight >= maxWeight)
        {
            GameObject.Find("Player(Clone)").GetComponent<PlayerController>().baseMoveSpeed = 0f;
        }
        else
        {
            GameObject.Find("Player(Clone)").GetComponent<PlayerController>().baseMoveSpeed = 5f;
        }
    }
}
