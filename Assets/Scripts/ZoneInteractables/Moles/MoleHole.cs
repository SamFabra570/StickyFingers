using UnityEngine;
using UnityEngine.AI;

public class MoleHole : MonoBehaviour
{
    [SerializeField] private float holeLifetime = 15f;
    [SerializeField] private float mapRange = 50f;
    
    private GameObject nearbyHole;
    private static bool holeCreated;
    private static Vector3 entryHolePos;
    private static Vector3 exitHolePos;

    private float timer;
    private bool playerInRange = false;
    private bool isTeleporting = false;
    private Transform playerTransform;
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
            return;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            isTeleporting = true;
            TeleportPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        playerTransform = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        playerTransform = null;
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
        Destroy(gameObject);
    }
}