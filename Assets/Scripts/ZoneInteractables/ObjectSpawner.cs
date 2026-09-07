// ObjectSpawner.cs

using UnityEngine;

namespace ZoneInteractables
{
    public class ObjectSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject prefabToSpawn;
        [SerializeField] private Transform spawnPoint; // if null, uses this object's position
        [SerializeField] private float spawnYOffset = 1f;
        [SerializeField] private bool spawnOnStart = false;

        private bool _shouldSpawn = false;
        
        [SerializeField] private ItemDatabase itemDatabase;

        public bool ShouldSpawn
        {
            get => _shouldSpawn;
            set
            {
                _shouldSpawn = value;
                if (_shouldSpawn)
                    SpawnObject();
            }
        }

        private void Start()
        {
            if (spawnOnStart)
                SpawnObject();
        }

        public void SpawnObject()
        {
            if (prefabToSpawn == null)
            {
                Debug.LogWarning("[ObjectSpawner] No prefab assigned.");
                return;
            }
            
            InventoryItemData item = itemDatabase.GetRandomItemByRarity(ItemRarity.Gold);

            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion rot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
            
            pos.y += spawnYOffset;

            GameObject spawnedObject = Instantiate(prefabToSpawn, pos, rot);
            
            spawnedObject.GetComponent<ItemController>().referenceItem = item;
            
            Destroy(gameObject);
        }

        /// <summary>
        /// Can be called from other scripts or Unity Events.
        /// </summary>
        public void TriggerSpawn() => ShouldSpawn = true;
    }
}