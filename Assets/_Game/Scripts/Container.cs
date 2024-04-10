using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Container : BaseInteractable
{
    #region Base Interactable
    public override InteractableType Type => InteractableType.Container;
    public override bool IsInteractable => isInteractable;
    public override bool IsHeld => isHeld;
    public override void OnHoverEnter()
    {
        // glow around object
    }
    public override void OnHoverExit()
    {
        // stop glow around object
    }
    public override void HandlePrimaryInteraction()
    {
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
    }
    public override void HandleSecondaryInteraction()
    {
        
    }
    public override void HandleHeldInteraction()
    {
        
    }
    #endregion

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

    public void Button_TakeItemOutOfContainer()
    {
        if (containedItem == null || currentAmount <= 0)
            return;

        IInteractable item = containedItemParent.GetChild(0).GetComponent<IInteractable>();
        Product product = (item as Product);
        product.OnInteract(InteractType.Primary);
        product.transform.parent = null;
        
        if (currentAmount <= 0)
        {
            containedItem = null;
            takeOutButton.SetActive(false);
        }

        UpdateDisplay();
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
