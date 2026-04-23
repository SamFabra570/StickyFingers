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

        CompassArrow arrowObj = PlayerController.Instance.arrow;
        
        if (arrowObj != null) 
            SetCompassArrow(arrowObj, randomPoint);
        else
        {
            Debug.Log("No CompassArrow found");
        }
        
        UIManager.Instance.ShowPortalSpawnNotif();
        Debug.Log("PORTAL SPAWNED PORTAL SPAWNED");
    }

    private void SetCompassArrow(CompassArrow arrow, Transform target)
    {
        arrow.SetActive(true);
        arrow.SetTarget(target.transform);
    }
}
