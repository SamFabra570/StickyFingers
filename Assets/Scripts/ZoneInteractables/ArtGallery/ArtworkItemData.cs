using UnityEngine;

public enum ArtworkType { Sculpture, Painting, Garment }

[CreateAssetMenu(fileName = "ArtworkItemData", menuName = "Inventory/ArtworkItemData")]
public class ArtworkItemData : InventoryItemData
{
    [Header("Artwork Settings")]
    public ArtworkType artworkType;
}
