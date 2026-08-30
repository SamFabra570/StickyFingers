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
        [Tooltip("How much awareness the burst dumps on everyone in range. ~70 is enough to make a guard come and look without making them certain.")]
        [SerializeField] private float awarenessBump = 70f;

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
            //This used to reach past every enemy's FSM and write agent_.SetDestination directly, which the
            //enemy's own Patrol state then overwrote on its very next LogicUpdate. The noise produced one
            //frame of tugging and nothing else. Feeding perception instead means the enemy's brain decides
            //to go and look — and it actually gets there.
            EnemyAlertNetwork.ReportNoise(transform.position, radius, awarenessBump);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
