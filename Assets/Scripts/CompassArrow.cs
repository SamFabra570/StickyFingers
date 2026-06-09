using System.Collections;
using UnityEngine;

public class CompassArrow : MonoBehaviour
{
    [SerializeField] private Transform arrowObj;
    [SerializeField] private Transform currentTarget;
    [SerializeField] private Material baseColour;
    private Color colour;
    [SerializeField] private float blinkSpeed;

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

        if (!value)
        {
            StopCoroutine(Blink());
        }
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

    public void ToggleBlinking(bool state)
    {
        if (state)
        {
            StartCoroutine(Blink());
        }
        else
        {
            StopCoroutine(Blink());
            
            colour.a = 1;
            baseColour.color = colour;
        }
    }
    
    //Routine for blinking feedback
    private IEnumerator Blink()
    {
        //colour = baseColour.color;
        
        while (true)
        {
            yield return new WaitForSeconds(blinkSpeed);
            
            arrowObj.gameObject.SetActive(false);

            //colour.a = 0;
            //baseColour.color = colour;
            
            yield return new WaitForSeconds(blinkSpeed);
            
            arrowObj.gameObject.SetActive(true);
            
            // colour.a = 1;
            // baseColour.color = colour;
        }
        
    }
}
