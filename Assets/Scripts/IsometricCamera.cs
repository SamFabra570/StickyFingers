using System;
using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    [SerializeField] private Transform pivot;
    
    [SerializeField] private float baseAngle = 135f;
    [SerializeField] private float stepAngle = 60f;
    [SerializeField] private int totalSteps = 6;

    private int currentStep;

    [SerializeField] private float rotationSpeed = 10f;
    private Quaternion targetRotation;
    
    private Vector3 offset;
    [SerializeField] private float smoothTime;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        offset = transform.position - target.position;
    }

    private void Start()
    {
        targetRotation = Quaternion.Euler(45f, baseAngle, 0f);
        pivot.rotation = targetRotation;
    }

    private void Update()
    {
        pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        //Move camera with player
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }

    public void RotateCamera(int direction)
    {
        currentStep = (currentStep + direction + totalSteps) % totalSteps;
        float yRotation = baseAngle + (currentStep * stepAngle);
        targetRotation = Quaternion.Euler(45f, yRotation, 0f);
    }
}
