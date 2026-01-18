using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static PlayerController Instance;
    
    [SerializeField] AbilityCooldownUI ability1UI;
    [SerializeField] AbilityCooldownUI ability2UI;
    [SerializeField] AbilityCooldownUI ability3UI;
    
    private float speed;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boostSpeed = 7f;
    [SerializeField] private float turnSpeed = 360f;

    private Vector2 inputData;
    private CharacterController controller;
    private Animator animator;

    private PlayerInput inputMap;
    private bool isPaused = false;

    public Smoke smoke;
    public GameObject smokeEmitter;

    [SerializeField] private GameObject forceField;
    [SerializeField] float freezeDuration = 2f;
    private bool isFrozen = false;

    private int interactType;
    
    private bool gogglesUp = false;

    private void Awake()
    {
        Instance = this;
        
        controller = gameObject.AddComponent<CharacterController>();
        animator = gameObject.GetComponent<Animator>();
        
        inputMap = new PlayerInput();

        inputMap.Player.Movement.performed += Movement_performed =>
        {
            inputData = Movement_performed.ReadValue<Vector2>();
        };
        
        inputMap.Player.Movement.canceled += Movement_canceled =>
        {
            inputData = Movement_canceled.ReadValue<Vector2>();
        };

        inputMap.Player.Sprint.performed += Sprint_performed =>
        {
            speed = boostSpeed;
        };

        inputMap.Player.Sprint.canceled += Sprint_canceled =>
        {
            speed = moveSpeed;
        };

        inputMap.Player.Pause.performed += Pause_performed =>
        {
            switch (isPaused)
            {
                case true:
                    isPaused = false;
                    UIManager.Instance.HideScreen("Pause");
                    break;
                case false:
                    isPaused = true;
                    UIManager.Instance.ShowScreen("Pause");
                    break;
            }
        };

        inputMap.Player.P_Animation.performed += P_Animation_performed =>
        {
            PlayGogglesAnim();
        };
        
        inputMap.Player.Interact.performed += Interact_performed =>
        {
            Interact(interactType);
        };
        
        inputMap.Player.Inventory.performed += Inventory_performed =>
        {
            switch (isPaused)
            {
                case true:
                    isPaused = false;
                    UIManager.Instance.HideScreen("Inventory");
                    break;
                case false:
                    isPaused = true;
                    UIManager.Instance.ShowScreen("Inventory");
                    break;
            }
        };

        inputMap.Player.Ability1.performed += Ability1_performed =>
        {
            AbilityManager.Instance.ActivateAbility(0);
            ability1UI.SetAbilityActive();
        };
        
        inputMap.Player.Ability2.performed += Ability2_performed =>
        {
            AbilityManager.Instance.ActivateAbility(1);
            ability2UI.SetAbilityActive();
        };
        
        inputMap.Player.Ability3.performed += Ability3_performed =>
        {
            AbilityManager.Instance.ActivateAbility(2);
            ability3UI.SetAbilityActive();
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

    private void Interact(int interactionType)
    {
        //Interact - object pickup
        if (interactionType == 0)
        {
            
        }

        //Interact - hide (closet)
        if (interactionType == 1)
        {
            
        }

        //Interact - NPC
        if (interactionType == 2)
        {
            
        }
        
        //Interact - Computer
        if (interactionType == 3)
        {
            
        }
        
        //Interact - Doors
        if (interactionType == 4)
        {
            
        }

        //Interact - interactable (distraction objects)
        if (interactionType == 5)
        {
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Object"))
        {
            interactType = 0;
        }
        
        if (other.CompareTag("Closet"))
        {
            interactType = 1;
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
