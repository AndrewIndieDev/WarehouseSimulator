using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Container;
    public Transform Transform => transform;
    public Collider Collider => collider;
    public Rigidbody Rigidbody => rb;
    public Transform Seat => null;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => isHeld;

    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isHeld = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private new Collider collider;

    [SerializeField] private IInteractable containedItem;
    [SerializeField] private int currentAmount;
    [SerializeField] private int maxStackSize;

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
            case InteractType.Place:
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
        if (containedItem == null)
        {
            containedItem = interactable;
            currentAmount = 1;
            return true;
        }
        else if ((containedItem as Product).productId == (interactable as Product).productId && currentAmount < maxStackSize)
        {
            currentAmount++;
            return true;
        }
        return false;
    }
}
