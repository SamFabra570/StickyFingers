using UnityEngine;
using ZoneInteractables;

public class BookShelf : MonoBehaviour, IInteractable
{
    [Header("Effects")]
    [SerializeField] private GameObject grassPatchPrefab;
    [SerializeField] private int grassPatchCount = 5;
    [SerializeField] private float grassSpawnRadius = 15f;
    [SerializeField] private float grassHeightOffset = 0.1f;
    [SerializeField] private float maxSpawnHeight = 1f;

    [SerializeField] private GameObject demonHelperPrefab;

    private bool _bookPlaced = false;

    public void Interact(GameObject player)
    {
        if (_bookPlaced) return;

        if (!FindBookInInventory(out BookItemData book))
        {
            Debug.Log("[BookShelf] No book in inventory.");
            return;
        }

        RemoveBookFromInventory(book);
        TriggerEffect(book.bookColor);
        _bookPlaced = true;
    }

    private void TriggerEffect(BookColor color)
    {
        switch (color)
        {
            case BookColor.Green:
                SpawnGrassPatches();
                break;
            case BookColor.Blue:
                if (RainController.Instance != null)
                    RainController.Instance.StartRain();
                else
                    Debug.LogWarning("[BookShelf] RainController not found in scene.");
                break;
            case BookColor.Red:
                SpawnDemon();
                break;
        }
    }

    private void SpawnGrassPatches()
    {
        if (grassPatchPrefab == null)
        {
            Debug.LogWarning("[BookShelf] GrassPatch prefab not assigned.");
            return;
        }

        int spawned = 0;
        int attempts = 0;
        int maxAttempts = grassPatchCount * 5;

        while (spawned < grassPatchCount && attempts < maxAttempts)
        {
            attempts++;

            if (!RandomNavMeshPoint.TryGetRandomPoint(grassSpawnRadius, out Vector3 navPos))
                continue;

            // Descartar puntos del NavMesh que estén demasiado alto (paredes/plataformas)
            if (navPos.y > maxSpawnHeight)
                continue;

            // RaycastAll para ignorar objetos encima del suelo y encontrar el piso real
            Vector3 rayOrigin = navPos + Vector3.up * 5f;
            RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, 10f);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            bool foundGround = false;
            float groundY = 0f;
            foreach (var hit in hits)
            {
                if (hit.point.y <= maxSpawnHeight)
                {
                    groundY = hit.point.y;
                    foundGround = true;
                    break;
                }
            }

            if (!foundGround) continue;

            navPos.y = groundY + grassHeightOffset;

            Instantiate(grassPatchPrefab, navPos, Quaternion.identity);
            spawned++;
        }
    }

    private void SpawnDemon()
    {
        if (demonHelperPrefab == null)
        {
            Debug.LogWarning("[BookShelf] DemonHelper prefab not assigned.");
            return;
        }
        Instantiate(demonHelperPrefab, transform.position, Quaternion.identity);
    }

    private bool FindBookInInventory(out BookItemData found)
    {
        found = null;
        var inventory = GetInventory();
        if (inventory == null) return false;

        foreach (var item in inventory.inventory)
        {
            if (item.data is BookItemData bookData)
            {
                found = bookData;
                return true;
            }
        }
        return false;
    }

    private void RemoveBookFromInventory(BookItemData book)
    {
        GetInventory().Remove(book);
    }

    private InventorySystem GetInventory()
    {
        var container = GameObject.Find("InventoryContainer")?.GetComponent<InventoryContainer>();
        if (container == null)
        {
            Debug.LogWarning("[BookShelf] InventoryContainer not found.");
            return null;
        }
        return container.inventorySystem;
    }
}
