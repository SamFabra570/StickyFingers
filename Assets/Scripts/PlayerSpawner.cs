using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu") 
            return;
        
        if (PlayerController.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Player");
            GameObject player = Instantiate(prefab);
            MovePlayerToSpawn(player.transform);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerController.Instance != null)
        {
            MovePlayerToSpawn(PlayerController.Instance.transform);
        }
    }

    private void MovePlayerToSpawn(Transform player)
    {
        GameObject spawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if (spawn != null)
        {
            player.position = spawn.transform.position;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
