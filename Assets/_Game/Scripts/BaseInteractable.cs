using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    public List<Component> disableOnGhostPlacement = new();

    public virtual InteractableType Type => InteractableType.None;

    public virtual bool IsInteractable => true;

    public virtual bool IsHeld => false;

    public virtual void OnHoverEnter()
    {

    }

    public virtual void OnHoverExit()
    {

    }

    public void OnInteract(InteractType type)
    {
        switch (type)
        {
            case InteractType.Primary:
                HandlePrimaryInteraction();
                break;
            case InteractType.Secondary:
                HandleSecondaryInteraction();
                break;
            case InteractType.HeldInteraction:
                HandleHeldInteraction();
                break;
            default:
                Debug.LogError($"InteractType <{type}> has not been added to this use case. . .");
                break;
        }
    }

    public virtual void HandlePrimaryInteraction()
    {
        
    }

    public virtual void HandleSecondaryInteraction()
    {
        
    }

    public virtual void HandleHeldInteraction()
    {
        
    }

    public virtual void ToggleComponents(bool enabled)
    {
        foreach (Component component in disableOnGhostPlacement)
        {
            MeshRenderer mr = (component as MeshRenderer);
            Collider col = (component as Collider);
            if (mr != null)
                mr.enabled = enabled;
            else if (col != null)
                col.enabled = enabled;
        }
    }
}
