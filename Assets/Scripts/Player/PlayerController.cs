using System;
using System.Collections;
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
    private float currentSpeed;
    private bool isSprinting;
    [SerializeField] public float baseMoveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float frozenFloorSpeed = 12f;
    public float abilityMoveSpeed;

    private Vector3 horizontalVelocity;
    
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float turnAccelerationBonus = 20f;
    
    [SerializeField] private float turnSpeed = 360f;

    [Header("Weight Mechanic")] 
    public WeightState currentState = WeightState.Light;
    
    public enum WeightState
    {
        Light,
        Medium,
        Heavy,
        Overweight
    }
    
    [SerializeField] private float currentWeight;
    
    public float mediumThreshold = 0.4f;
    public float heavyThreshold = 0.75f;

    //Value to subtract from speed
    public float mediumSpeedModifier = -1;
    public float heavySpeedModifier = -2;
    
    [Header ("Player Gravity")]
    public bool useGravity = true;
    public float yVelocity;
    [SerializeField] private float gravityMultiplier = 1f;

    private Vector2 inputData;
    private CharacterController controller;
    private Animator animator;
    
    //Camera
    [Header ("Camera Refs")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private IsometricCamera cameraScript;

    public PlayerInput inputMap;
    public bool isPaused;
    [HideInInspector] public bool isInvOpen;
    [HideInInspector] public bool isPauseOpen;

    [Header ("Freeze Player Checks")]
    [SerializeField] private GameObject stunField;
    //[SerializeField] float freezeDuration = 2f;
    public bool isFrozen;
    private bool isFloorFrozen;

    [Header ("Ability Checks")]
    public GameObject wings;
    public GameObject vacuumZone;
    public GameObject forceField;

    public bool hasUsedSecondChance;

    public bool isInvisible;

    public Renderer rend;
    
    public Material basePlayerMat;

    private ItemController objectToSteal;
    private GameObject interactable;
    private int interactType;
    
    public CompassArrow arrow;

    public ButtonMash buttonMashObj;
    
    private bool gogglesUp;
    public InventoryItemData inventoryItem;
    private Collider detectedEnemy; 
    
    private CharacterController cc;
    private SoundPlayer soundPlayer;

    // How many enemy sensors currently see the player. >0 = detected (drives the detection vignette).
    private int _detectorCount;
    public bool IsDetected => _detectorCount > 0;

    // Soft cover: 0 = exposed, up to PlayerCover.maxConcealment while standing in cover zones.
    // Enemies read this to shrink their detection range; the scout also slows its suspicion build-up.
    private PlayerCover _playerCover;
    public float Concealment => _playerCover != null ? _playerCover.Concealment : 0f;

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
        _playerCover = GetComponent<PlayerCover>();

        //cameraScript = FindFirstObjectByType<IsometricCamera>();
        
        inputMap = new PlayerInput();
        //inputMap = GetComponent<PlayerInput>();

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
            isSprinting = true;
        };

        inputMap.Player.Sprint.canceled += Sprint_canceled =>
        {
            isSprinting = false;
        };

        inputMap.Player.Pause.performed += Pause_performed =>
        {
            if (!isPaused)
            {
                if (!isPauseOpen)
                {
                    UIManager.Instance.OpenMenu("PauseMenu");
                }
                else
                {
                    UIManager.Instance.HideMenu();
                }
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
                //Stealable Object
                case 0:
                    if (currentWeight <= 1)
                    {
                        StealObject();
                        UIManager.Instance.ToggleInteractText(false, "");
                    }
                    else
                        Debug.Log("UR SO FAT U CANT EVEN STEAL ANYMORE");
                    break;
                    
                //Interactable Object
                case 1:
                    Interact(interactable);
                    break;
                //Mash Event
                case 2:
                    buttonMashObj.MashEvent();
                    break;
            }
        };
        
        inputMap.Player.Inventory.performed += Inventory_performed =>
        {
            if (SceneManager.GetActiveScene().name != ("Game"))
                return;

            if (!isPaused)
            {
                if (!isInvOpen)
                {
                    UIManager.Instance.OpenMenu("InventoryMenu");
                }
                else
                {
                    UIManager.Instance.HideMenu();
                }
            }
        };

        inputMap.Player.ResetCooldown.performed += ResetCooldown_performed =>
        {
            AbilityManager.Instance.ResetAbilityCooldowns();
        };

        inputMap.Player.Ability1.performed += Ability1_performed =>
        {
            if (SceneManager.GetActiveScene().name != ("Game"))
                return;
            
            AbilityManager.Instance.ActivateAbility(0);
            feedback.GetAbilitySlot(0);
        };
        
        inputMap.Player.Ability2.performed += Ability2_performed =>
        {
            if (SceneManager.GetActiveScene().name != ("Game"))
                return;
            
            AbilityManager.Instance.ActivateAbility(1);
            feedback.GetAbilitySlot(1);
        };
        
        inputMap.Player.Ability3.performed += Ability3_performed =>
        {
            if (SceneManager.GetActiveScene().name != ("Game"))
                return;
            
            AbilityManager.Instance.ActivateAbility(2);
            feedback.GetAbilitySlot(2);
        };
        
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        soundPlayer = GetComponentInChildren<SoundPlayer>();
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
        SetWeightState();
        UpdateSpeed();
        
        bool isMoving = cc.velocity.sqrMagnitude > 0.0001f;
        if (soundPlayer != null)
        {
            if (!isMoving)
                soundPlayer.distance_ = 0f;

            detectedEnemy = soundPlayer.detected_object_;
            if (detectedEnemy != null)
            {
                if (detectedEnemy.TryGetComponent(out BaseEnemy baseEnemy))
                    baseEnemy.agent_.SetDestination(transform.position);
                else if (detectedEnemy.TryGetComponent(out BaseScoutEnemy scout))
                    scout.agent_.SetDestination(transform.position);
                else if (detectedEnemy.TryGetComponent(out BaseMageEnemy mage))
                    mage.agent_.SetDestination(transform.position);
            }
        }

        if (!isFrozen)
            CorrectMovement();
        
        Look();
    }

    private void SetWeightState()
    {
        //Weight on a scale from 0-1
        currentWeight = UIManager.Instance.normalizedWeight;
        
        //Light
        if (currentWeight < mediumThreshold) //Less than medium
        {
            currentState = WeightState.Light;
        }
        //Medium
        else if (currentWeight >= mediumThreshold && currentWeight <= heavyThreshold) //More than medium, less than heavy
        {
            currentState = WeightState.Medium;
            //currentSpeed = sprintSpeed + encumberedSpeedModifier;
        }
        //Heavy
        else if (currentWeight > heavyThreshold && currentWeight < 1) //More than heavy, less than maximum
        {
            currentState = WeightState.Heavy;
            //currentSpeed = sprintSpeed + overencumberedSpeedModifier;
        }
        else if (currentWeight >= 1) //Greater or equal to maximum
            currentState = WeightState.Overweight;
    }

    private void UpdateSpeed()
    {
        float weightModifier = 0;
        
        switch (currentState)
        {
            case WeightState.Light:
                weightModifier = 0;
                break;
            case WeightState.Medium:
                weightModifier = mediumSpeedModifier;
                break;
            case WeightState.Heavy:
                weightModifier = heavySpeedModifier;
                break;
            case WeightState.Overweight:
                currentSpeed = 0;
                return;
        }

        if (SceneManager.GetActiveScene().name != "Game")
            weightModifier = 0;

        //If ability is active, override speed
        if (abilityMoveSpeed > 0)
        {
            currentSpeed = abilityMoveSpeed;
            acceleration = 40;
            deceleration = 50;
            return;
        }
        
        if (isSprinting) //Sprinting
        {
            currentSpeed = sprintSpeed + weightModifier;
            
            float sprintRadius = RainController.Instance != null && RainController.Instance.IsRaining ? 2f : 4f;
            if (soundPlayer != null) 
                soundPlayer.distance_ = sprintRadius;
        }
        else if (isFloorFrozen) //Frozen Floor
        {
            currentSpeed = frozenFloorSpeed;
            acceleration = 5;
            deceleration = 1f;
        }
        else //Base
            currentSpeed = baseMoveSpeed + weightModifier;
        
        //Base acceleration settings
        acceleration = 40;
        deceleration = 50;
        
        if (soundPlayer != null) 
            soundPlayer.distance_ = 2f;
        
        //Rigidbody rb = GetComponent<Rigidbody>();

        if (SceneManager.GetActiveScene().name != "Game")
            isFloorFrozen = false;
    }

    private void CorrectMovement()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "Post-Game") 
            return;
        
        Vector3 rawDir = new Vector3(inputData.x, 0, inputData.y);
        
        //Camera rotated 135 degrees for isometric view
        Quaternion camRotation = Quaternion.Euler(0, cameraPivot.eulerAngles.y, 0);
        Vector3 correctedDir = camRotation * rawDir;
        correctedDir.y = 0;
        
        float inputMagnitude = Mathf.Clamp01(correctedDir.magnitude);
        if (inputMagnitude > 0f)
            correctedDir = correctedDir.normalized * inputMagnitude;
    
        //Acceleration system
        Vector3 targetVelocity = correctedDir * currentSpeed;

        float turnDot = 0f;
        if (horizontalVelocity.sqrMagnitude > 0.01f && correctedDir != Vector3.zero)
        {
            turnDot = Vector3.Dot(horizontalVelocity.normalized, correctedDir);
        }

        float currentAcceleration = acceleration;
    
        //Snappy turns
        if (turnDot < 0.5f)
            currentAcceleration += turnAccelerationBonus;

        if (correctedDir != Vector3.zero)
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, 
                currentAcceleration * Time.deltaTime);
        }
        else
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero,
                deceleration * Time.deltaTime);
        }
    
        //Rotation based on velocity
        if (horizontalVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);

            float dynamicRotationSpeed = rotationSpeed;

            if (turnDot < 0.3f)
                dynamicRotationSpeed *= 1.5f;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                dynamicRotationSpeed * Time.deltaTime);
        }
    
        //Add base gravity
        if (controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        if (useGravity) 
            yVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
    
        //Final movement
        Vector3 moveDelta = horizontalVelocity;
        moveDelta.y = yVelocity;
        controller.Move(moveDelta * Time.deltaTime);
    }

    public void FreezeMovement(float freezeDuration)
    {
        if (isFrozen)
            return;

        StartCoroutine(FreezePlayer(freezeDuration));
    }

    public void ToggleIcyFloor(bool isFloorIcy)
    {
        switch (isFloorIcy)
        {
            case true:
                Debug.Log("slippy slidey");
                isFloorFrozen = true;
                break;
            case false:
                Debug.Log("back 2 normal");
                isFloorFrozen = false;
                break;
        }
    }

    private IEnumerator FreezePlayer(float freezeDur)
    {
        isFrozen = true;

        if (stunField != null)
        {
            stunField.transform.position = transform.position;
            stunField.SetActive(true);
        }
        
        yield return new WaitForSeconds(freezeDur);

        if (stunField != null)
        {
            stunField.SetActive(false);
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

    public void MovePlayerUp(float amount)
    {
        controller.Move(Vector3.up * amount);
    }

    //Called by enemy Sight sensors when they start/stop seeing the player.
    public void AddDetector() => _detectorCount++;
    public void RemoveDetector() => _detectorCount = Mathf.Max(0, _detectorCount - 1);

    private void OnTriggerEnter(Collider other)
    {
        //Add object outline
        if (other.CompareTag("Object"))
        {
            interactType = 0;
            objectToSteal = other.GetComponent<ItemController>();
            
            UIManager.Instance.ShowPreviewItem(objectToSteal.referenceItem);
            UIManager.Instance.ToggleInteractText(true, other.tag);
        }
        else if (other.CompareTag("Interactable"))
        {
            interactType = 1;
            interactable = other.gameObject;
            
            UIManager.Instance.ToggleInteractText(true, other.tag);
            //Debug.Log("Showing interact text");
        }
        else if (other.CompareTag("MashEvent"))
        {
            interactType = 2;
            interactable = other.gameObject;

            buttonMashObj = interactable.GetComponent<ButtonMash>();
            UIManager.Instance.ToggleInteractText(true, other.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Object"))
        {
            UIManager.Instance.DisablePreview();
            objectToSteal = null;
            
        }
        if (other.CompareTag("Interactable"))
        {
            interactable = null;
        }

        if (other.CompareTag("MashEvent"))
        {
            interactable = null;
            buttonMashObj = null;
        }
        
        UIManager.Instance.ToggleInteractText(false, "");
    }

    private void StealObject()
    {
        //Add object to inventory
        objectToSteal.Pickup();
        GetComponent<PlayerSoundController>()?.PlaySteal();
        UIManager.Instance.DisablePreview();
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

        Debug.Log("Find camera, scene: " + SceneManager.GetActiveScene().name);
        
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        
        if (spawnPoint != null)
            transform.position = spawnPoint.transform.position;
    
        //Rebind camera on spawn
        cameraScript = FindFirstObjectByType<IsometricCamera>();

        arrow = GetComponentInChildren<CompassArrow>(true);
        
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
                portalClass.PlayActivate();

                if (SceneManager.GetActiveScene().name == "Game")
                {
                    arrow.SetActive(false);
                    
                    Debug.Log("End Game FROM PORTAL");
                    GameManager.Instance.EndGame(true, "");
                }
                
                if (SceneManager.GetActiveScene().name == "HUB")
                    GameManager.Instance.StartGame();
            }

            return;
        }
        
        if (obj.TryGetComponent(out InteractableHandler type))
        {
            if (type == (type.interactableType == Interactables.PlanningDesk))
            {
                UIManager.Instance.OpenMenu("LoadoutMenu");
                FreezeMovement(0);
                
            }
            
            if (type == (type.interactableType == Interactables.ProgressionDesk))
            {
                UIManager.Instance.OpenMenu("ProgressionMenu");
                FreezeMovement(0);
            }
        }

        if (obj.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(gameObject);
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
