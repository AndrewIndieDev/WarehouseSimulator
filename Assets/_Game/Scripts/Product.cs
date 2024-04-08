using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Texture2D ProductIcon { get { return productIcon;  } set { productIcon = value; } }

    public string productId;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isHeld = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private new Collider collider;
    [SerializeField] private Texture2D productIcon;

    public void OnInteract(InteractType interactType)
    {
        switch (interactType)
        {
            case InteractType.Default:
                if (!isHeld) // If the object we are interacting with is not held
                {
                    isHeld = true;
                    FreezeProduct();
                }
                else // If the object we are interacting with is held
                {
                    isHeld = false;
                    UnFreezeProduct();
                }
                break;
            case InteractType.PlaceInContainer:
                isHeld = false;
                rb.isKinematic = true;
                Collider.enabled = false;
                break;
        }
    }

    public void FreezeProduct()
    {
        rb.isKinematic = true;
        Collider.enabled = false;
    }

    public void UnFreezeProduct()
    {
        rb.isKinematic = false;
        Collider.enabled = true;
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
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = Vector3.zero;
        FreezeProduct();
    }

    public void TakeOut()
    {
        transform.localScale = Vector3.one;
        transform.parent = null;
        //transform.SetLocalPositionAndRotation(transform.position + Transform.up * 0.5f, Quaternion.identity);
        isHeld = false;
        UnFreezeProduct();
    }
    
    protected virtual void Start()
    {
        ProductManager.Instance.AddProductCount(productId, 1);
    }

    private void Update()
    {
        if (transform.position.y < -10)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = GameManager.Instance.RespawnPosition;
        }
    }

    private void OnDestroy()
    {
        ProductManager.Instance.RemoveProductCount(productId, 1);
    }
}
