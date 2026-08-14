using UnityEngine;
using UnityEngine.AI;

public class MoleHole : MonoBehaviour, IInteractable
{
    [SerializeField] private float holeLifetime = 15f;
    [SerializeField] private float mapRange = 50f;
    
    private GameObject nearbyHole;
    private static bool holeCreated;
    private static Vector3 entryHolePos;
    private static Vector3 exitHolePos;

    private float timer;
    public bool playerInRange;
    private bool isTeleporting;
    public Transform playerTransform;
    public GameObject moleHolePrefab;

    private void OnEnable()
    {
        timer = holeLifetime;
    }

    private void Update()
    {
        if (isTeleporting) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            CloseHole();
        }
    }

    public void Interact(GameObject player)
    {
        if (playerInRange)
        {
            isTeleporting = true;
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        
        cc.enabled = false;
        
        if (holeCreated)
        {
            Vector3 nearbyHolePos = transform.position;
            nearbyHolePos.y += cc.height / 2f + cc.skinWidth;
            
            Debug.Log("Teleporting to existing holes");
            if (entryHolePos == nearbyHolePos)
            {
                Debug.Log("Trying to TP");
                playerTransform.position = exitHolePos;
            }
            else if (exitHolePos == nearbyHolePos)
            {
                Debug.Log("Trying to TP");
                playerTransform.position = entryHolePos;
            }
        }
        else
        {
            if (RandomNavMeshPoint.TryGetRandomPoint(mapRange, out Vector3 destination))
            {
                Debug.Log("Creating new hole");
                holeCreated = true;
                
                entryHolePos = transform.position;
                entryHolePos.y += cc.height / 2f + cc.skinWidth;
            
                Instantiate(moleHolePrefab, destination, Quaternion.identity);
            
                destination.y += cc.height / 2f + cc.skinWidth;
                exitHolePos = destination;
                
                playerTransform.position = exitHolePos;
                //CloseHole();
            }
            else
            {
                Debug.LogWarning("MoleHole: Could not find a valid NavMesh point to teleport to.");
            }
        }
        
        cc.enabled = true;
        isTeleporting = false;
    }

    private void CloseHole()
    {
        playerInRange = false;
        playerTransform = null;
        
        Destroy(gameObject);
    }
}