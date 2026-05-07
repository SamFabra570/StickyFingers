using System.Collections.Generic;
using UnityEngine;
using ZoneInteractables;

public class CauldronController : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private ObjectSpawner potionSpawner;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;

    [Header("Noise on Fail")]
    [SerializeField] private NoiseEmitter noiseEmitter;

    private readonly List<IngredientItemData> _addedIngredients = new List<IngredientItemData>();
    private bool _brewed = false;

    public void Interact(GameObject player)
    {
        if (_brewed) return;

        var inventory = GetInventory();
        if (inventory == null) return;

        IngredientItemData ingredient = FindIngredientInInventory(inventory);
        if (ingredient == null)
        {
            Debug.Log("[Cauldron] No ingredient in inventory.");
            return;
        }

        inventory.Remove(ingredient);
        _addedIngredients.Add(ingredient);

        Debug.Log($"[Cauldron] Added: {ingredient.ingredientType}. Total: {_addedIngredients.Count}/3");

        if (_addedIngredients.Count >= 3)
            Brew();
    }

    private void Brew()
    {
        _brewed = true;

        if (IsCorrectCombo())
        {
            PlaySound(successSound);
            if (potionSpawner != null)
                potionSpawner.TriggerSpawn();
            else
                Debug.LogWarning("[Cauldron] No potion spawner assigned.");
        }
        else
        {
            PlaySound(failSound);
            if (noiseEmitter != null)
                noiseEmitter.AlertNearbyEnemies();
            else
                Debug.LogWarning("[Cauldron] No NoiseEmitter assigned.");
        }
    }

    private bool IsCorrectCombo()
    {
        bool hasHerb = false;
        bool hasLiquid = false;
        bool hasPowder = false;

        foreach (var ingredient in _addedIngredients)
        {
            switch (ingredient.ingredientType)
            {
                case IngredientType.Herb:   hasHerb = true;   break;
                case IngredientType.Liquid: hasLiquid = true; break;
                case IngredientType.Powder: hasPowder = true; break;
            }
        }

        return hasHerb && hasLiquid && hasPowder;
    }

    private IngredientItemData FindIngredientInInventory(InventorySystem inventory)
    {
        foreach (var item in inventory.inventory)
        {
            if (item.data is IngredientItemData ingredient)
                return ingredient;
        }
        return null;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private InventorySystem GetInventory()
    {
        var container = GameObject.Find("InventoryContainer")?.GetComponent<InventoryContainer>();
        if (container == null)
        {
            Debug.LogWarning("[Cauldron] InventoryContainer not found.");
            return null;
        }
        return container.inventorySystem;
    }
}
