using UnityEngine;

using UnityEngine;

public class MoleController : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private GameObject moleHolePrefab;

    private bool hasBeenScared = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenScared) return;
        if (!other.CompareTag("Player")) return;

        GetScared();
    }

    private void GetScared()
    {
        hasBeenScared = true;

        Instantiate(moleHolePrefab, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
}