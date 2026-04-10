// PressurePlateController.cs
using UnityEngine;
using ZoneInteractables;

public class PressurePlateController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObjectSpawner objectSpawner;

    [Header("Sink Settings")]
    [SerializeField] private float sinkDistance = 0.5f;
    [SerializeField] private float sinkDuration = 1.5f;

    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered || !other.CompareTag("Player")) return;
        if (objectSpawner == null)
        {
            Debug.LogWarning("[PressurePlateController] No ObjectSpawner assigned.");
            return;
        }
        var inventory = GameObject.Find("InventoryContainer")
            .GetComponent<InventoryContainer>()
            .inventorySystem;

        float normalizedWeight = inventory.totalWeight / GameManager.Instance.maxWeight;

        if (normalizedWeight <= 0.66f) return;
        _triggered = true;
        objectSpawner.TriggerSpawn();
        StartCoroutine(SinkAndDestroy());
    }

    private System.Collections.IEnumerator SinkAndDestroy()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos - new Vector3(0f, sinkDistance, 0f);
        float elapsed = 0f;

        while (elapsed < sinkDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / sinkDuration);
            yield return null;
        }

        transform.position = targetPos;
        Destroy(gameObject);
    }
}