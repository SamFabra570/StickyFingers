using UnityEngine;

public enum IngredientType { Herb, Liquid, Powder }

[CreateAssetMenu(fileName = "IngredientItemData", menuName = "Inventory/IngredientItemData")]
public class IngredientItemData : InventoryItemData
{
    [Header("Ingredient Settings")]
    public IngredientType ingredientType;
}
