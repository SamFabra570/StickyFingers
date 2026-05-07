using UnityEngine;

public enum BookColor { Green, Blue, Red }

[CreateAssetMenu(fileName = "BookItemData", menuName = "Inventory/BookItemData")]
public class BookItemData : InventoryItemData
{
    [Header("Book Settings")]
    public BookColor bookColor;
}
