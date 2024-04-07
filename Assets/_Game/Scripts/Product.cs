using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Pickup;
    public Transform Transform => transform;
    public Collider Collider => collider;
    public Rigidbody Rigidbody => rb;
    public Transform Seat => null;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => isHeld;
    public List<Component> DisableOnPlacement => null;

    public string productId;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isHeld = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private new Collider collider;

    public void OnInteract(InteractType interactType)
    {
        switch (interactType)
        {
            case InteractType.Default:
                if (!isHeld) // If the object we are interacting with is not held
                {
                    isHeld = true;
                    rb.isKinematic = true;
                    Collider.enabled = false;
                }
                else // If the object we are interacting with is held
                {
                    isHeld = false;
                    rb.isKinematic = false;
                    Collider.enabled = true;
                }
                break;
            case InteractType.PlaceInContainer:
                isHeld = false;
                rb.isKinematic = true;
                Collider.enabled = false;
                break;
        }
    }

    public void OnHoverEnter()
    {
        // glow around object
    }

    public void OnHoverExit()
    {
        // stop glow around object
    }

    public void PutInside(Transform newParent)
    {
        transform.parent = newParent;
        transform.SetPositionAndRotation(newParent.position, newParent.rotation);
        transform.localScale = Vector3.zero;
    }

    public void TakeOut()
    {
        transform.localScale = Vector3.one;
        transform.parent = null;
        transform.SetLocalPositionAndRotation(transform.position + Transform.up * 0.5f, Quaternion.identity);
        isHeld = false;
        rb.isKinematic = false;
        Collider.enabled = true;
    }
}
