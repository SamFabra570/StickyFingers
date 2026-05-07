using UnityEngine;

namespace ZoneInteractables
{
    /// <summary>
    /// Emits a noise burst from a fixed world position, alerting nearby enemies to investigate.
    /// Used by mechanics that attract guards without involving the player directly
    /// (e.g. cauldron fart, gallery smell).
    /// </summary>
    public class NoiseEmitter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float radius = 5f;
        [SerializeField] private float duration = -1f; // -1 = stays until manually destroyed

        private float _timer;

        private void OnEnable()
        {
            _timer = duration;
            AlertNearbyEnemies();
        }

        private void Update()
        {
            if (duration < 0f) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                Destroy(gameObject);
        }

        public void AlertNearbyEnemies()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            foreach (var hit in hits)
                TrySendEnemy(hit.gameObject);
        }

        private void TrySendEnemy(GameObject obj)
        {
            if (obj.GetComponentInParent<BaseEnemy>() is BaseEnemy baseEnemy)
                baseEnemy.agent_.SetDestination(transform.position);
            else if (obj.GetComponentInParent<BaseScoutEnemy>() is BaseScoutEnemy scout)
                scout.agent_.SetDestination(transform.position);
            else if (obj.GetComponentInParent<BaseMageEnemy>() is BaseMageEnemy mage)
                mage.agent_.SetDestination(transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
