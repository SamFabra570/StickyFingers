using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform player;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam == null)
            cam = Camera.main;
        
        //transform.position = player.position + Vector3.up * 1f;

        //transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        
        transform.forward = cam.transform.forward;
    }
}
