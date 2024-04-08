using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

public class Container : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Container;
    public Transform Transform => transform;
    public Collider Collider => collider;
    public Rigidbody Rigidbody => rb;
    public Transform Seat => null;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => isHeld;
    public List<Component> DisableOnPlacement => disableOnPlacement;
    public IInteractable ContainedItem => containedItem;
    public int MaxStackSize => maxStackSize;
    public int CurrentAmount => currentAmount;

    [HideInInspector] public UnityEvent<Container> OnPickupContainer;

    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isHeld = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private new Collider collider;
    [SerializeField] private GameObject takeOutButton;
    [SerializeField] private Transform containedItemParent;
    [SerializeField] private IInteractable containedItem;
    [SerializeField] private int currentAmount => containedItemParent.childCount;
    [SerializeField] private int maxStackSize;
    [SerializeField] private List<Component> disableOnPlacement = new();
    [SerializeField] private Transform displayParent;

    public void OnInteract(InteractType interactType)
    {
        switch (interactType)
        {
            case InteractType.Default:
                if (!isHeld) // If the object we are interacting with is not held
                {
                    isHeld = true;
                    FreezeContainer();
                    OnPickupContainer?.Invoke(this);
                }
                else // If the object we are interacting with is held
                {
                    isHeld = false;
                    UnFreezeContainer();
                }
                break;
            case InteractType.PlaceInContainer:
                // Do nothing, something is being placed in it.
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

    public bool PlaceItemInContainer(IInteractable interactable)
    {
        Product product = (interactable as Product);
        if (containedItem == null)
        {
            takeOutButton.SetActive(true);
            containedItem = interactable;
            UpdateDisplay();
            product.PutInside(containedItemParent);
            return true;
        }
        else if ((containedItem as Product).productId == product.productId && currentAmount < maxStackSize)
        {
            product.PutInside(containedItemParent);
            UpdateDisplay();
            return true;
        }
        return false;
    }

    public IInteractable TakeItemOutOfContainer()
    {
        if (containedItem == null || currentAmount <= 0)
            return null;

        IInteractable item = containedItemParent.GetChild(0).GetComponent<IInteractable>();
        Product product = (item as Product);
        product.TakeOut();
        
        if (currentAmount <= 0)
        {
            containedItem = null;
            takeOutButton.SetActive(false);
        }

        UpdateDisplay();
        return item;
    }

    private void UpdateDisplay()
    {
        List<MeshRenderer> meshRenderers = displayParent.GetComponentsInChildren<MeshRenderer>(true).ToList();
        if (containedItem != null)
        {
            foreach (MeshRenderer mr in meshRenderers)
            {
                if ((containedItem as Product).ProductIcon != null)
                {
                    mr.material.SetTexture("_MainTex", (containedItem as Product).ProductIcon);
                }
            }
            displayParent.gameObject.SetActive(true);
        }
        else
        {
            displayParent.gameObject.SetActive(false);
        }
    }

    public void FreezeContainer()
    {
        rb.isKinematic = true;
        Collider.enabled = false;
    }

    public void UnFreezeContainer()
    {
        rb.isKinematic = false;
        Collider.enabled = true;
    }
}
