using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    [SerializeField] Transform[] portalPoints;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int count = transform.childCount;
        portalPoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            portalPoints[i] = transform.GetChild(i);
        }
    }

    public void SpawnPortal()
    {
        GameObject portalPrefab = Resources.Load<GameObject>("Portal");
        
        Transform randomPoint = portalPoints[Random.Range(0, portalPoints.Length)];
        Instantiate(portalPrefab, randomPoint.position, randomPoint.rotation);
        
        Debug.Log("PORTAL SPAWNED PORTAL SPAWNED");
    }
}
