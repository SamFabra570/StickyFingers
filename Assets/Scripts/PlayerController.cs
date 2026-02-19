using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private AbilityFeedback feedback;
    
    [Header ("Ability Refs")]
    [SerializeField] AbilityCooldownUI ability1UI;
    [SerializeField] AbilityCooldownUI ability2UI;
    [SerializeField] AbilityCooldownUI ability3UI;
    
    [Header ("Player Movement")]
    public float speed = 3.5f; //m/s
    public float heightOffset;
    private float lastHeightOffset;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float boostSpeed = 7f;
    [SerializeField] private float turnSpeed = 360f;

    private Vector2 inputData;
    private CharacterController controller;
    private Animator animator;
    
    //Camera
    [Header ("Camera Refs")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private IsometricCamera cameraScript;

    private PlayerInput inputMap;
    private bool isPaused;

    [Header ("Freeze Player Checks")]
    [SerializeField] private GameObject forceField;
    [SerializeField] float freezeDuration = 2f;
    public bool isFrozen;

    [Header ("Ability Checks")]
    public GameObject wings;

    public bool isInvisible;

    public Renderer rend;
    
    public Material basePlayerMat;

    private ItemController objectToSteal;
    private GameObject interactable;
    private int interactType;   
    
    private bool gogglesUp;
    public InventoryItemData inventoryItem;
    public bool portalCharged = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        
        if (SceneManager.GetActiveScene().name != "GameTesting") 
            DontDestroyOnLoad(this);
        
        controller = gameObject.AddComponent<CharacterController>();
        animator = gameObject.GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        feedback = GetComponent<AbilityFeedback>();
        
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
                    StealObject();
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
            feedback.GetAbilitySlot(0);
        };
        
        inputMap.Player.Ability2.performed += Ability2_performed =>
        {
            AbilityManager.Instance.ActivateAbility(1);
            feedback.GetAbilitySlot(1);
        };
        
        inputMap.Player.Ability3.performed += Ability3_performed =>
        {
            AbilityManager.Instance.ActivateAbility(2);
            feedback.GetAbilitySlot(2);
        };
        
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = moveSpeed;
        forceField.SetActive(false);
        wings.SetActive(false);

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        
        interactable = null;
        objectToSteal = null;
    }

    // Update is called once per frame
    void Update()
    {
        CorrectMovement();
        Look();
    }

    private void CorrectMovement()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "Post-Game") 
            return;
        
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
            objectToSteal = other.GetComponent<ItemController>();
            
            UIManager.Instance.showPreviewItem(objectToSteal.referenceItem);
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
            UIManager.Instance.disablePreview();
            objectToSteal = null;
            
        }
        if (other.CompareTag("Interactable"))
        {
            interactable = null;
        }
    }

    private void StealObject()
    {
        //Add object to inventory
        objectToSteal.Pickup();
        UIManager.Instance.disablePreview();
        Debug.Log("Add " + objectToSteal.name + " to inventory");
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        inputMap.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name == "MainMenu") 
            return;
        
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        
        if (spawnPoint != null)
            transform.position = spawnPoint.transform.position;
    
        //Rebind camera on spawn
        cameraScript = FindFirstObjectByType<IsometricCamera>();
        
        if (cameraScript != null)
        {
            cameraPivot = cameraScript.transform;
            cameraScript.SetRefs(transform, cameraPivot);
        }
        else
        {
            Debug.LogWarning("Camera not found in scene!");
        }
    }

    private void Interact(GameObject obj)
    {
        if (obj.TryGetComponent(out ExitPortal portalClass))
        {
            if (portalClass.state == PortalState.Charged)
            {
                if (SceneManager.GetActiveScene().name == "Game")
                {
                    Debug.Log("End Game FROM PORTAL");
                    GameManager.Instance.EndGame();
                }
                    
                
                if (SceneManager.GetActiveScene().name == "HUB")
                    GameManager.Instance.StartGame();
            }

            return;
        }
        
        if (obj.name == "PlanningDesk")
        {
            HUB_UIManager.Instance.TogglePlanningUI("Show");
            FreezeMovement();
        }
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

    public void ResetColour()
    {
        rend.material = basePlayerMat;
    }

    public void SetPlayerColour(Material abilityColour)
    {
        rend.material = abilityColour;
    }
    
}
