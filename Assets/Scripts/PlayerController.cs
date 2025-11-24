using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 inputData;
    private CharacterController controller;

    private PlayerInput inputMap;

    private void Awake()
    {
        controller = gameObject.AddComponent<CharacterController>();
        
        inputMap = new PlayerInput();

        inputMap.Player.Movement.performed += Movement_performed =>
        {
            inputData = Movement_performed.ReadValue<Vector2>();
        };
        
        inputMap.Player.Movement.canceled += Movement_canceled =>
        {
            inputData = Movement_canceled.ReadValue<Vector2>();
        };
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CorrectMovement();
        Look();
    }

    private void CorrectMovement()
    {
        Vector3 rawDir = new Vector3(inputData.x, 0, inputData.y);
        
        //Camera rotated 135 degrees for isometric view
        Quaternion camRotation = Quaternion.Euler(0, 135, 0);

        //Rotate direction so input fits isometric camera
        Vector3 correctedDir = camRotation * rawDir;
        correctedDir.y = 0;
        
        if (correctedDir.magnitude > 1f)
            correctedDir.Normalize();
        
        controller.Move(correctedDir * moveSpeed * Time.deltaTime);
    }

    private void Look()
    {
        if (inputData != Vector2.zero)
        {
            var relative = (transform.position + new Vector3(inputData.x, 0, inputData.y)) - transform.position;
            var rotation = Quaternion.LookRotation(relative, Vector3.up);
        
            transform.rotation = rotation;
        }
    }
    
    private void OnEnable()
    {
        inputMap.Enable();
    }

    private void OnDisable()
    {
        inputMap.Disable();
    }
}
