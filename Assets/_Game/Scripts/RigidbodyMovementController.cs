using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMovementController : MonoBehaviour
{
    public Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;

    public Camera cam;
    public float movementSpeed = 10.0f;
    public float mouseSensitivityX = 5.0f;
    public float mouseSensitivityY = 5.0f;
    public float sprintMultiplier = 2.0f;

    private float currentYaw;
    private float currentPitch;

    private bool hasControl = true;
    private List<string> currentControlTakers = new List<string>();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        if (!hasControl) return;

        bool isSprinting = Input.GetButton("Sprint");

        Vector3 forward = cam.transform.forward.IgnoreAxis(EIgnoreAxis.Y).normalized;
        Vector3 right = cam.transform.right.IgnoreAxis(EIgnoreAxis.Y).normalized;

        Vector3 movementVector = horizontalInput * right + verticalInput * forward;
        movementVector.Normalize();

        rb.AddForce(movementVector * (movementSpeed * (isSprinting ? sprintMultiplier : 1.0f)));
    }

    private void Update()
    {
        if (!hasControl) return;

        currentYaw += Input.GetAxisRaw("Mouse X") * mouseSensitivityX;
        currentPitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
        currentPitch = Math.Clamp(currentPitch, -89.99f, 89.99f);
        transform.rotation = Quaternion.Euler(0.0f, currentYaw, 0.0f);
        cam.transform.localRotation = Quaternion.Euler(currentPitch, 0.0f, 0.0f);

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
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
}
