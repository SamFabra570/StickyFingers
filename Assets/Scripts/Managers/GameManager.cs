using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public HashSet<string> completedTutorials = new();
    public HashSet<string> completedMissions = new();

    public PassiveAbilityController PlayerPassives;

    public float maxDebt;
    public float remainingDebt;
    public float startingDebt;
    public float maxWeight;
    public float deeperPocketsWeight = 500;
    //public bool runState;

    public bool successfulRun;
    public string endRunState;
    public float extractedBounty;
    public float timeRemaining;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        Instance = this;
        
        if (startingDebt != 0) 
            remainingDebt = startingDebt;
        else 
            remainingDebt = maxDebt;
            
    }
    public void PauseGame(int pauseState)
    {
        switch (pauseState)
        {
            case 0:
                Time.timeScale = 1;
                PlayerController.Instance.isPaused = false;
                break;
            case 1:
                Time.timeScale = 0;
                PlayerController.Instance.isPaused = true;
                break;
        }
    }

    public void StartGame()
    {
        // TEMPORAL: apunta a Game2 para probar ZoneInteractables en desarrollo.
        // Revertir a "Game" cuando todas las mecánicas estén implementadas y probadas.
        StartCoroutine(LoadRoutine("Game"));
        //SceneManager.LoadScene("Game2");

        if (PlayerPassives.Has(PassiveAbilities.DeeperPockets))
        {
            maxWeight = deeperPocketsWeight;
            Debug.Log("Endless Pockets activated. Max Weight: " + maxWeight);
        }

        if (PlayerPassives.Has(PassiveAbilities.SecondChance))
            PlayerController.Instance.hasUsedSecondChance = false;
        
        AbilityManager.Instance.ResetAbilityCooldowns();
    }

    public void EndGame(bool hasExtracted, string endType)
    {
        successfulRun = hasExtracted;
        endRunState = endType;
        
        InventoryContainer inv = GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>();
        
        AbilityManager.Instance.InterruptAllAbilities();

        if (successfulRun)
        {
            extractedBounty = inv.inventorySystem.totalBounty;
            timeRemaining = TimeManager.Instance.remainingTime;
        }
        else
        {
            extractedBounty = 0;
        }
        
        inv.inventorySystem.SellInventory(successfulRun);
        
        SceneManager.LoadScene("Post-Game");
    }

    public float GetDebtPaidPercent()
    {
        float normalizedDebt = remainingDebt / maxDebt;
        
        return 1 - normalizedDebt;
    }
    
    private IEnumerator LoadRoutine(string sceneName)
    {
        LoadingUI.Instance.Show();

        yield return null; // lets UI appear for 1 frame

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        // small buffer so it doesn't feel instant
        yield return new WaitForSeconds(0.2f);

        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }

        LoadingUI.Instance.Hide();
    }
}
