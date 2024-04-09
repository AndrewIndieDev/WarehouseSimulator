using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Container : BaseInteractable, IInteractable
{
    public InteractableType Type => InteractableType.Container;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => isHeld;
    public IInteractable ContainedItem => containedItem;
    public int MaxStackSize => maxStackSize;
    public int CurrentAmount => currentAmount;

    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isHeld = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private new Collider collider;
    [SerializeField] private GameObject takeOutButton;
    [SerializeField] private Transform containedItemParent;
    [SerializeField] private IInteractable containedItem;
    [SerializeField] private int currentAmount => containedItemParent.childCount;
    [SerializeField] private int maxStackSize;
    [SerializeField] private Transform displayParent;

    public void OnInteract(InteractType type)
    {
        switch (type)
        {
            case InteractType.Primary:
                if (!isHeld) // If the object we are interacting with is not held
                {
                    isHeld = true;
                    FreezeContainer();
                }
                else // If the object we are interacting with is held
                {
                    isHeld = false;
                    UnFreezeContainer();
                }
                break;
            case InteractType.Secondary:
                break;
            default:
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
            return true;
        }
        else if ((containedItem as Product).productId == product.productId && currentAmount < maxStackSize)
        {
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
        ToggleComponents(false);
    }

    public void UnFreezeContainer()
    {
        rb.isKinematic = false;
        ToggleComponents(true);
    }
}
