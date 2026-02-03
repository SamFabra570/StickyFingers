using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    [SerializeField] AbilityCooldownUI ability1UI;
    [SerializeField] AbilityCooldownUI ability2UI;
    [SerializeField] AbilityCooldownUI ability3UI;
    
    
    public float speed = 1f; //m/s
    public float heightOffset;
    private float lastHeightOffset;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boostSpeed = 7f;
    [SerializeField] private float turnSpeed = 360f;

    private Vector2 inputData;
    private CharacterController controller;
    private Animator animator;
    
    //Camera
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private IsometricCamera cameraScript;

    private PlayerInput inputMap;
    private bool isPaused;

    [SerializeField] private GameObject forceField;
    [SerializeField] float freezeDuration = 2f;
    public bool isFrozen;

    public GameObject wings;

    public bool isInvisible;

    private Renderer rend;
    
    public Material basePlayerMat;
    public Material intangibleMat;
    public Material invisibleMat;

    private GameObject objectToSteal;
    private GameObject interactable;
    private int interactType;   
    
    private bool gogglesUp;
    public InventoryItemData inventoryItem;
    
    private void Awake()
    {
        Instance = this;
        
        controller = gameObject.AddComponent<CharacterController>();
        animator = gameObject.GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        
        inputMap = new PlayerInput();

        inputMap.Player.Movement.performed += Movement_performed =>
        {
            inputData = Movement_performed.ReadValue<Vector2>();
        };
        
        inputMap.Player.Movement.canceled += Movement_canceled =>
        {
            inputData = Movement_canceled.ReadValue<Vector2>();
        };

        inputMap.Player.RotateCameraLeft.performed += RotateCameraLeft_performed =>
        {
            cameraScript.RotateCamera(1);
        };
        
        inputMap.Player.RotateCameraRight.performed += RotateCameraLeft_performed =>
        {
            cameraScript.RotateCamera(-1);
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
            if (objectToSteal == null && interactable == null)
                return;
            
            switch (interactType)
            {
                case 0:
                    StealObject(objectToSteal);
                    Debug.Log("Steal object");
                    break;
                case 1:
                    Interact(interactable);
                    Debug.Log("Interact");
                    break;
            }
            
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

        inputMap.Player.ResetCooldown.performed += ResetCooldown_performed =>
        {
            AbilityManager.Instance.ResetAbilityCooldowns();
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
        speed *= moveSpeed;
        forceField.SetActive(false);
        wings.SetActive(false);
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
        Quaternion camRotation = Quaternion.Euler(0, cameraPivot.eulerAngles.y, 0);

        //Rotate direction so input fits isometric camera
        Vector3 correctedDir = camRotation * rawDir;
        correctedDir.y = 0;
        
        if (correctedDir.magnitude > 1f)
            correctedDir.Normalize();

        //Save base move data
        Vector3 moveDelta = correctedDir * speed * Time.deltaTime;
        
        //Calculate height offset and apply to base move
        float heightDelta = heightOffset - lastHeightOffset;
        Vector3 heightMove = Vector3.up * heightDelta;
        
        //Final movement
        controller.Move(moveDelta + heightMove);
        
        lastHeightOffset = heightOffset;
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

    private void OnTriggerEnter(Collider other)
    {
        //Add object outline
        if (other.CompareTag("Object"))
        {
            interactType = 0;
            objectToSteal = other.gameObject;
        }
        else if (other.CompareTag("Interactable"))
        {
            interactType = 1;
            interactable = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Object"))
        {
            objectToSteal = null;
        }
        if (other.CompareTag("Interactable"))
        {
            interactable = null;
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

    public void Interact(GameObject obj)
    {
        if (obj.name == "Portal")
        {
            SceneManager.LoadScene("Game");
        }
        else if (obj.name == "PlanningDesk")
        {
            HUB_UIManager.Instance.TogglePlanningUI(1);
            FreezeMovement();
        }
    }

    private void StealObject(GameObject obj)
    {
        //Add object to inventory
        Debug.Log("Add " + obj.name + " to inventory");
        obj.SetActive(false);
    }

    public void ActivatePhase()
    {
        gameObject.layer = LayerMask.NameToLayer("Intangible");
        rend.material = intangibleMat;
    }

    public void DeactivatePhase()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        rend.material = basePlayerMat;
    }
    
    public void ActivateInvisibility()
    {
        isInvisible = true;
        rend.material = invisibleMat;
    }
    
    public void DeactivateInvisibility()
    {
        isInvisible = false;
        rend.material = basePlayerMat;
    }

    public void ActivateShield()
    {
        forceField.transform.position = transform.position;
        forceField.SetActive(true);
    }

    public void DeactivateShield()
    {
        forceField.SetActive(false);
    }

    
}
