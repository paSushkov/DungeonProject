using System;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public delegate void AxisInputProcessor(float horizontal, float vertical);
    public delegate void MouseMoveProcessor(float X, float Y);
    public delegate void ShootProcessor();
    
    public event AxisInputProcessor AxisInputDone;
    public event MouseMoveProcessor MouseInputDone;
    public event ShootProcessor ShootInputDone;
    
    private float horizintalInput;
    private float verticalInput;
    private bool emptyAxisWasSent;   
    
    private float mouseXMoveInput;
    private float mouseYMoveInput;
    private bool emptyMouseMoveWasSent;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        ReadAxisInput();
        BroadcastAxisInput();

        ReadMouseMoveInput();
        BroadcastMouseMoveInput();

        ReadAndBroadCastShoot();
    }
    private void ReadAxisInput()
    {
        horizintalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void BroadcastAxisInput()
    {
        if (horizintalInput != 0 || verticalInput != 0)
        {
            AxisInputDone?.Invoke(horizintalInput, verticalInput);
            emptyAxisWasSent = false;
        }
        else if (!emptyAxisWasSent)
        {
            AxisInputDone?.Invoke(0f, 0f);
            emptyAxisWasSent = true;
        }
    }
    
    private void ReadMouseMoveInput()
    {
            mouseXMoveInput = Input.GetAxis("Mouse X");
            mouseYMoveInput = Input.GetAxis("Mouse Y");
    }
    
    private void BroadcastMouseMoveInput()
    {
        if (mouseXMoveInput != 0 || mouseXMoveInput != 0)
        {
            MouseInputDone?.Invoke(mouseXMoveInput, mouseYMoveInput);
            emptyMouseMoveWasSent = false;
        }
        else if (!emptyMouseMoveWasSent)
        {
            MouseInputDone?.Invoke(0f, 0f);
            emptyMouseMoveWasSent = true;
        }
    }

    private void ReadAndBroadCastShoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootInputDone?.Invoke();
        }

    }
}
