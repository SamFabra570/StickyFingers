using System.Collections;
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
    private Animator animator;

    private PlayerInput inputMap;

    public Smoke smoke;
    public GameObject smokeEmitter;

    [SerializeField] private GameObject forceField;
    [SerializeField] float freezeDuration = 2f;
    
    private bool isFrozen = false;
    
    private bool gogglesUp = false;

    private void Awake()
    {
        controller = gameObject.AddComponent<CharacterController>();
        animator = gameObject.GetComponent<Animator>();
        
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

        inputMap.Player.Pause.performed += Pause_performed =>
        {
            UIManager.Instance.ShowPauseScreen();
        };

        inputMap.Player.P_Animation.performed += P_Animation_performed =>
        {
            PlayGogglesAnim();
        };
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = moveSpeed;
        forceField.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CorrectMovement();
        Look();
    }

    private void CorrectMovement()
    {
        if (isFrozen)
            return;
        
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

    public void FreezeMovement()
    {
        if (isFrozen)
            return;

        StartCoroutine(FreezePlayer());
    }

    private IEnumerator FreezePlayer()
    {
        isFrozen = true;

        if (forceField != null)
        {
            forceField.transform.position = transform.position;
            forceField.SetActive(true);
        }
        
        yield return new WaitForSeconds(freezeDuration);

        if (forceField != null)
        {
            forceField.SetActive(false);
        }
        
        isFrozen = false;
    }

    private void PlayGogglesAnim()
    {
        if (!gogglesUp)
        {
            gogglesUp = true;
            animator.SetBool("P_Trigger", true);
        }
        
        else if (gogglesUp)
        {
            gogglesUp = false;
            animator.SetBool("P_Trigger", false);
        }
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
