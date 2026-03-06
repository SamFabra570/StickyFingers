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

    public Transform mageSpawn;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject mage;

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
        //Debug.Log("Game Over: YOU LOSE");
        Destroy(mage);
        GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem.SellInventory(runState);
        SceneManager.LoadScene("Post-Game");
        runState = false;
    }

    public void SpawnMage()
    {
        GameObject prefab = Resources.Load<GameObject>("Mage");
        mage = Instantiate(prefab);
        mage.transform.position = mageSpawn.position;
        
        UIManager.Instance.ShowMageSpawnNotif();
    }
}
