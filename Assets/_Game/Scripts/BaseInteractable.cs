using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseInteractable : NetworkBehaviour, IInteractable
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

    
    public void OnInteract(InteractType type, ulong sender)
    {
        OnInteractServerRPC(type, sender);
    }

    [ServerRpc(RequireOwnership = false)]
    protected virtual void OnInteractServerRPC(InteractType type, ulong sender)
    {
        NetworkObject.ChangeOwnership(sender);
        OnInteractClientRPC(type, sender);
    }
    
    [ClientRpc]
    protected virtual void OnInteractClientRPC(InteractType type, ulong sender)
    {
        switch (type)
        {
            case InteractType.Primary:
                HandlePrimaryInteraction(sender);
                break;
            case InteractType.Secondary:
                HandleSecondaryInteraction(sender);
                break;
            case InteractType.HeldInteraction:
                HandleHeldInteraction(sender);
                break;
            default:
                Debug.LogError($"InteractType <{type}> has not been added to this use case. . .");
                break;
        }
    }

    protected virtual void HandlePrimaryInteraction(ulong sender)
    {
        
    }

    protected virtual void HandleSecondaryInteraction(ulong sender)
    {
        
    }

    protected virtual void HandleHeldInteraction(ulong sender)
    {
        
    }

    protected virtual void ToggleComponents(bool isEnabled)
    {
        foreach (Component component in disableOnGhostPlacement)
        {
            MeshRenderer mr = (component as MeshRenderer);
            Collider col = (component as Collider);
            if (mr != null)
                mr.enabled = isEnabled;
            else if (col != null)
                col.enabled = isEnabled;
        }
    }
}
