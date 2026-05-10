using UnityEngine;

public class CompassArrow : MonoBehaviour
{
    [SerializeField] private Transform arrowObj;
    [SerializeField] private Transform currentTarget;
    [SerializeField] private Material baseColour;

    private bool isActive;

    // Update is called once per frame
    void Update()
    {
        if (currentTarget == null)
            return;
        
        if (!isActive &&  currentTarget != null)
            SetActive(true);
    
        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) 
            return;

        if (direction.magnitude < 2.5f)
            SetActive(false);
            
        arrowObj.forward = Vector3.Slerp(arrowObj.forward, direction.normalized, Time.deltaTime * 10f);
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    public void SetActive(bool value)
    {
        isActive = value;
        arrowObj.gameObject.SetActive(value);
    }

    public void SetColour(Material colour)
    {
        if (colour != null)
        {
            arrowObj.GetComponentInChildren<MeshRenderer>().material = colour;
            
            return;
        }
        
        arrowObj.GetComponentInChildren<MeshRenderer>().material = baseColour;
    }
}
