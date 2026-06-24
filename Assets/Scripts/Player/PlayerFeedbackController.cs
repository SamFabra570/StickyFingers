using UnityEngine;

public class PlayerFeedbackController : MonoBehaviour
{
    private PlayerController player;
    private Camera cam;
    
    private float targetZoom;

    [Header("Zoom Settings")] 
    [SerializeField] private float zoomVelocity;
    
    [Header ("Base Settings")]
    [SerializeField] private float baseZoom;
    
    [Header ("Modifiers")]
    [SerializeField] private float sprintZoom;
    [SerializeField] private float mediumZoom;
    [SerializeField] private float heavyZoom;
    [SerializeField] private float overweightZoom;
    [SerializeField] private float stealthZoom;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        if (cam != null) 
            baseZoom = cam.orthographicSize;
        
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            baseZoom = cam.orthographicSize;
        }

        UpdateFOV();
        UpdatePostPro();
    }

    private void UpdateFOV()
    {
        targetZoom =
            baseZoom +
            //GetSprintModifier() +
            GetWeightModifier();
            //GetStealthModifier();

        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref zoomVelocity, 0.4f);
    }

    private void UpdatePostPro()
    {
        
    }

    float GetWeightModifier()
    {
        return player.currentState switch
        {
            PlayerController.WeightState.Light => 0f,
            PlayerController.WeightState.Medium => mediumZoom,
            PlayerController.WeightState.Heavy => heavyZoom,
            PlayerController.WeightState.Overweight => overweightZoom
        };
    }
    
    float GetSprintModifier()
    {
        return player.isSprinting ? sprintZoom : 0f;
    }
    
    //Add this for when enemies are close
    // float GetStealthModifier()
    // {
    //     return player.IsStealth ? -0.5f : 0f;
    // }
}
