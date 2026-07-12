using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputType
{
    Keyboard,
    Xbox,
    PlayStation
}

public class InputIconManager : MonoBehaviour
{
    public static InputIconManager Instance { get; private set; }

    public static event Action<InputType> OnInputTypeChanged;

    [SerializeField] private PlayerInput playerInput;

    public InputType CurrentInputType { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        playerInput.onControlsChanged += OnControlsChanged;

        // Set the correct icons immediately.
        DetectCurrentInput();
    }

    private void OnDisable()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        DetectCurrentInput();
    }

    private void DetectCurrentInput()
    {
        InputType newType;

        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            newType = InputType.Keyboard;
        }
        else
        {
            Gamepad gamepad = Gamepad.current;

            if (gamepad != null)
            {
                string manufacturer = gamepad.description.manufacturer;
                string displayName = gamepad.displayName;

                if (manufacturer.Contains("Sony", StringComparison.OrdinalIgnoreCase) ||
                    displayName.Contains("Dual", StringComparison.OrdinalIgnoreCase) ||
                    displayName.Contains("PlayStation", StringComparison.OrdinalIgnoreCase))
                {
                    newType = InputType.PlayStation;
                }
                else
                {
                    newType = InputType.Xbox;
                }
            }
            else
            {
                newType = InputType.Keyboard;
            }
        }

        if (newType == CurrentInputType)
            return;

        CurrentInputType = newType;
        OnInputTypeChanged?.Invoke(CurrentInputType);
    }
}