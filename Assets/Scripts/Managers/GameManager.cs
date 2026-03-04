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

    public float totalDebt;

    private void Awake()
    {
        /*if (instance == null)
        {
            //initialize the players inventory
            instance = this;
            instance.eggsRecovered=0;
        }
        else if (instance != this)
        {
            instance.eggsRecovered=0;
            instance.textComponent = this.textComponent;
        }
        DontDestroyOnLoad(instance);*/ 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        
        Instance.inventorySystem = new InventorySystem();
        Instance.inventorySystem.itemSlots = itemSlots;
        Instance.inventorySystem.itemDescriptionImage = itemDescriptionImage;
        Instance.inventorySystem.itemDescriptionNameText = itemDescriptionNameText;
        Instance.inventorySystem.itemDescriptionText = itemDescriptionText;
        Instance.totalDebt = 10000;

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
        SceneManager.LoadScene("Post-Game");
    }
}
