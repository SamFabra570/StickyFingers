using UnityEngine;

public class DanceArrow : MonoBehaviour
{
    [SerializeField] private int sequenceIndex;
    [SerializeField] private DanceFloorManager danceFloorManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        danceFloorManager.OnArrowStepped(sequenceIndex);
    }
}
