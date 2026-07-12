using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

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

    public InputType CurrentInputType { get; private set; } = InputType.Keyboard;

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
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // Ignore events that aren't actual state changes
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        InputType newType;

        if (device is Keyboard || device is Mouse)
        {
            newType = InputType.Keyboard;
        }
        else if (device is Gamepad gamepad)
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
            return;
        }

        if (newType == CurrentInputType)
            return;

        CurrentInputType = newType;
        OnInputTypeChanged?.Invoke(CurrentInputType);
    }
    
    public void RefreshIcons()
    {
        foreach (InputIconImage icon in FindObjectsByType<InputIconImage>(FindObjectsSortMode.None))
        {
            icon.UpdateIcon(CurrentInputType);
        }
    }
}