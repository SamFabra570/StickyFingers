using UnityEngine;
using ZoneInteractables;

public class ArtDisplay : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] public ArtworkType expectedType;

    [Header("References")]
    [SerializeField] private ArtGalleryManager galleryManager;

    public ArtworkItemData PlacedArtwork { get; private set; }
    public bool HasArtwork => PlacedArtwork != null;

    public void Interact(GameObject player)
    {
        if (HasArtwork)
        {
            Debug.Log("[ArtDisplay] Display already occupied.");
            return;
        }

        var inventory = GetInventory();
        if (inventory == null) return;

        ArtworkItemData artwork = FindMatchingArtworkInInventory(inventory);
        if (artwork == null)
        {
            Debug.Log($"[ArtDisplay] No {expectedType} in inventory.");
            return;
        }

        inventory.Remove(artwork);
        Place(artwork);
        galleryManager.OnArtworkPlaced();
    }

    private void Place(ArtworkItemData artwork)
    {
        PlacedArtwork = artwork;

        if (artwork.prefab != null)
            Instantiate(artwork.prefab, transform.position, transform.rotation);
    }

    private ArtworkItemData FindMatchingArtworkInInventory(InventorySystem inventory)
    {
        foreach (var item in inventory.inventory)
        {
            if (item.data is ArtworkItemData artworkData && artworkData.artworkType == expectedType)
                return artworkData;
        }
        return null;
    }

    private InventorySystem GetInventory()
    {
        var container = GameObject.Find("InventoryContainer")?.GetComponent<InventoryContainer>();
        if (container == null)
        {
            Debug.LogWarning("[ArtDisplay] InventoryContainer not found.");
            return null;
        }
        return container.inventorySystem;
    }
}
