using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float speed;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boostSpeed = 7f;
    [SerializeField] private float turnSpeed = 360f;

    private Vector2 inputData;
    private CharacterController controller;

    private PlayerInput inputMap;

    public Smoke smoke;
    public GameObject smokeEmitter;

    private void Awake()
    {
        controller = gameObject.AddComponent<CharacterController>();
        smoke = GetComponent<Smoke>();
        
        inputMap = new PlayerInput();

        inputMap.Player.Movement.performed += Movement_performed =>
        {
            inputData = Movement_performed.ReadValue<Vector2>();
        };
        
        inputMap.Player.Movement.canceled += Movement_canceled =>
        {
            inputData = Movement_canceled.ReadValue<Vector2>();
        };

        inputMap.Player.Boost.performed += Boost_performed =>
        {
            speed = boostSpeed;
        };

        inputMap.Player.Boost.canceled += Boost_canceled =>
        {
            speed = moveSpeed;
        };

        inputMap.Player.Smoke.performed += Smoke_performed =>
        {
            VFXManager.Instance.SpawnSmoke();
            //Play smoking animation
            //freeze player controls for time
        };
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = moveSpeed;
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
        
        controller.Move(correctedDir * speed * Time.deltaTime);
    }

    private void Look()
    {
        if (inputData != Vector2.zero)
        {
            var relative = (transform.position + new Vector3(inputData.x, 0, inputData.y)) - transform.position;
            var rotation = Quaternion.LookRotation(relative, Vector3.up);
        
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed * Time.deltaTime);
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
