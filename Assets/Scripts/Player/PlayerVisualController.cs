using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Camera cam;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite sideSprite;

    private void Update()
    {
        UpdateSprite();
        FaceCamera();
    }

    private void UpdateSprite()
    {
        Vector3 movementDirection = player.MovementDirection;

        // Don't change the sprite while stationary.
        if (movementDirection.sqrMagnitude < 0.01f)
            return;
        
        if (cam == null) 
            cam = Camera.main;

        // Only use the camera's Y rotation.
        Quaternion cameraRotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);

        // Convert world movement into camera-relative movement.
        Vector3 localDirection = Quaternion.Inverse(cameraRotation) * movementDirection;

        // Determine whether we're moving more horizontally or vertically
        // relative to the camera.
        if (Mathf.Abs(localDirection.x) > Mathf.Abs(localDirection.z))
        {
            // Sideways movement
            spriteRenderer.sprite = sideSprite;

            // Flip depending on which direction we're moving.
            spriteRenderer.flipX = localDirection.x > 0f;
        }
        else
        {
            // Forward/backward movement
            if (localDirection.z < 0f)
            {
                spriteRenderer.sprite = frontSprite;
            }
            else
            {
                spriteRenderer.sprite = backSprite;
            }

            spriteRenderer.flipX = false;
        }
    }
    
    private void FaceCamera()
    {
        if (cam == null) 
            cam = Camera.main;

        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        transform.rotation = Quaternion.LookRotation(-cameraForward);

    }
}
