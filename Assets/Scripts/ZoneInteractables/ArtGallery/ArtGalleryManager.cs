using UnityEngine;
using ZoneInteractables;

public class ArtGalleryManager : MonoBehaviour
{
    [Header("Displays")]
    [SerializeField] private ArtDisplay[] displays;

    [Header("References")]
    [SerializeField] private ObjectSpawner rewardSpawner;
    [SerializeField] private GameObject smellNoiseEmitterPrefab;

    [Header("Smell Duration")]
    [SerializeField] private float smellDuration = 90f;

    private bool _evaluated = false;

    public void OnArtworkPlaced()
    {
        if (_evaluated) return;

        foreach (var display in displays)
        {
            if (!display.HasArtwork) return;
        }

        Evaluate();
    }

    private void Evaluate()
    {
        _evaluated = true;

        if (IsCorrect())
        {
            Debug.Log("[ArtGallery] ¡Que galería más hermosa!");
            if (rewardSpawner != null)
                rewardSpawner.TriggerSpawn();
            else
                Debug.LogWarning("[ArtGalleryManager] No reward spawner assigned.");
        }
        else
        {
            Debug.Log("[ArtGallery] Esto es lo que pienso de tu galería...");
            SpawnSmell();
        }
    }

    private bool IsCorrect()
    {
        foreach (var display in displays)
        {
            if (display.PlacedArtwork == null) return false;
            if (display.PlacedArtwork.artworkType != display.expectedType) return false;
        }
        return true;
    }

    private void SpawnSmell()
    {
        if (smellNoiseEmitterPrefab == null)
        {
            Debug.LogWarning("[ArtGalleryManager] No smell emitter prefab assigned.");
            return;
        }

        var emitterObj = Instantiate(smellNoiseEmitterPrefab, transform.position, Quaternion.identity);

        if (emitterObj.TryGetComponent(out NoiseEmitter emitter))
        {
            // Override duration via the serialized field isn't possible at runtime from outside,
            // so we destroy it manually after smellDuration
            Destroy(emitterObj, smellDuration);
        }
    }
}
