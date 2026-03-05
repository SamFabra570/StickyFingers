using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public ItemSlot[] itemSlots;
    public InventorySystem inventorySystem;
    
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public Sprite emptySprite;

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
            Instance.inventorySystem = new InventorySystem();
            Instance.inventorySystem.itemSlots = itemSlots;
            Instance.inventorySystem.itemDescriptionImage = itemDescriptionImage;
            Instance.inventorySystem.itemDescriptionNameText = itemDescriptionNameText;
            Instance.inventorySystem.itemDescriptionText = itemDescriptionText;
            Instance.totalDebt = 10000;
            Instance.maxDebt = 10000;
            Instance.maxWeight = 50;
            Instance.itemDescriptionImage.sprite = emptySprite;
            Instance.runState = false;
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
        Debug.Log("Game Over: YOU LOSE");
        Instance.inventorySystem.SellInventory(runState);
        Debug.Log(GameManager.Instance.totalDebt);
        SceneManager.LoadScene("Post-Game");
    }
}
