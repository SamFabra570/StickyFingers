using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputRouter : MonoBehaviour
{
    public static UIInputRouter Instance;

    public InputActionReference navigate;
    public InputActionReference submit;
    public InputActionReference cancel;

    public Action<Vector2> onNavigate;
    public Action onSubmit;
    public Action onCancel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        navigate.action.performed += HandleNavigate;
        submit.action.performed += HandleSubmit;
        cancel.action.performed += HandleCancel;
        
        navigate.action.Enable();
        submit.action.Enable();
        cancel.action.Enable();
    }

    private void OnDisable()
    {
        navigate.action.performed -= HandleNavigate;
        submit.action.performed -= HandleSubmit;
        cancel.action.performed -= HandleCancel;

        navigate.action.Disable();
        submit.action.Disable();
        cancel.action.Disable();
    }
    
    private void HandleNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();
        onNavigate?.Invoke(dir);
    }

    private void HandleSubmit(InputAction.CallbackContext ctx)
    {
        onSubmit?.Invoke();
    }

    private void HandleCancel(InputAction.CallbackContext ctx)
    {
        onCancel?.Invoke();
    }
}
