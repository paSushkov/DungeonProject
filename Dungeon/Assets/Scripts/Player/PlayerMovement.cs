using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: refactor - implement Velocities !!! Player should gain speed at least on deep fall !!!
// ... despite the fact he has no place to fall...
public class PlayerMovement : MonoBehaviour
{
    public float ySensitivity = 1f;
    public float xSensitivity = 1f;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float downLookClampdown = -45f;
    [SerializeField] private float upLookClamp = 90f;
    [SerializeField] Camera mainCamera;
    private Transform mainCameraTransform;
    private float horizontalInput;
    private float verticalInput;
    private float xMouseInput;
    private float yMouseInput;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector3 playerGravityForce;

    void Start()
    {
        playerGravityForce = Physics.gravity;
        InputManager.Instance.AxisInputDone += GetAxisInput;
        InputManager.Instance.MouseInputDone += GetMouseMoveInput;
        mainCamera.TryGetComponent(out mainCameraTransform);
    }

    private void GetAxisInput(float horizontalInput, float verticalInput)
    {
        this.horizontalInput = horizontalInput;
        this.verticalInput = verticalInput;
    }

    private void GetMouseMoveInput(float xMouseInput, float yMouseInput)
    {
        this.xMouseInput = xMouseInput;
        this.yMouseInput = yMouseInput;
    }

    private void FixedUpdate()
    {
        ProcessMovement();
        ApplySimpleGravity();
    }

    private void LateUpdate()
    {
        ProcessLook();
    }

    private void ProcessMovement()
    {
        if (horizontalInput == 0 && verticalInput == 0) return;
        var direction = transform.forward * (verticalInput * walkSpeed) + 
            transform.right * (horizontalInput * walkSpeed);
            
            
            new Vector3(horizontalInput * walkSpeed, 0, verticalInput * walkSpeed);
        direction = Vector3.ClampMagnitude(direction, walkSpeed);

        characterController.Move(direction * Time.deltaTime);
    }

    private void ProcessLook()
    {
        if (yMouseInput != 0)
        {
            rotationX += yMouseInput * ySensitivity;
            rotationX = Mathf.Clamp(rotationX,downLookClampdown, upLookClamp);
            var localEulerAngles = mainCameraTransform.localEulerAngles;
            localEulerAngles
                = new Vector3(-rotationX, 
                    localEulerAngles.y,
                    localEulerAngles.z);
            mainCameraTransform.localEulerAngles = localEulerAngles;
        }

        if (xMouseInput != 0)
        {
            rotationY = xMouseInput * xSensitivity;
            transform.Rotate(0, rotationY, 0);
        }
    }
    
    private void ApplySimpleGravity()
    {
        characterController.Move(playerGravityForce * Time.deltaTime);
    }

}