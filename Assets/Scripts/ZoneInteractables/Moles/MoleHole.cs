using UnityEngine;
using UnityEngine.AI;

public class MoleHole : MonoBehaviour
{
    [SerializeField] private float holeLifetime = 15f;
    [SerializeField] private float mapRange = 50f;

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
        if (RandomNavMeshPoint.TryGetRandomPoint(mapRange, out Vector3 destination))
        {
            isTeleporting = false;
            
            CharacterController cc = playerTransform.GetComponent<CharacterController>();
            
            Instantiate(moleHolePrefab, destination, Quaternion.identity);
            
            destination.y += cc.height / 2f + cc.skinWidth;
            cc.enabled = false;
            playerTransform.position = destination;
            cc.enabled = true;
            //CloseHole();
        }
        else
        {
            Debug.LogWarning("MoleHole: Could not find a valid NavMesh point to teleport to.");
            isTeleporting = false;
        }
    }

    private void CloseHole()
    {
        Destroy(gameObject);
    }
}