using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    public Camera cam;
    public float movementSpeed = 5.0f;
    public float mouseSensitivityX = 5.0f;
    public float mouseSensitivityY = 5.0f;
    public float sprintMultiplier = 2.0f;
    //public float jumpStrength = 5.0f;

    private CharacterController cc;
    private Vector3 currentMovementVector;
    private float currentYaw;
    private float currentPitch;
    private float currentYVelocity;

    private bool hasControl = true;
    private List<string> currentControlTakers = new List<string>();
    
    
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AddControlTaker(string id)
    {
        currentControlTakers.Add(id);
        hasControl = currentControlTakers.Count <= 0;
        Cursor.lockState = hasControl ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hasControl;
    }
    
    public void RemoveControlTaker(string id)
    {
        currentControlTakers.Remove(id);
        hasControl = currentControlTakers.Count <= 0;
        Cursor.lockState = hasControl ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hasControl;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasControl) return;
        
        currentYaw += Input.GetAxisRaw("Mouse X") * mouseSensitivityX;
        currentPitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
        bool isSprinting = Input.GetButton("Sprint");
        currentPitch = Math.Clamp(currentPitch, -89.99f, 89.99f);
        transform.rotation = Quaternion.Euler(0.0f, currentYaw, 0.0f);
        cam.transform.localRotation = Quaternion.Euler(currentPitch, 0.0f, 0.0f);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 forward = cam.transform.forward.IgnoreAxis(EIgnoreAxis.Y).normalized;
        Vector3 right = cam.transform.right.IgnoreAxis(EIgnoreAxis.Y).normalized;

        Vector3 movementVector = horizontalInput * right + verticalInput * forward;
        movementVector.Normalize();

        cc.Move(movementVector * (Time.deltaTime * movementSpeed * (isSprinting ? sprintMultiplier : 1.0f)));
        cc.Move(Vector3.up * (currentYVelocity * Time.deltaTime));

        if (cc.isGrounded)
            currentYVelocity = 0.0f;
        else
            currentYVelocity += Physics.gravity.y * Time.deltaTime;

        //if (Input.GetButtonDown("Jump") && currentYVelocity <= 0.0f && currentYVelocity > -1.0f)
        //    currentYVelocity = jumpStrength;
    }
}
