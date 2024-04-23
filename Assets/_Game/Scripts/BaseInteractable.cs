using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseInteractable : NetworkBehaviour, IInteractable
{
    public List<Component> disableOnGhostPlacement = new();
    public virtual InteractableType Type => InteractableType.None;
    public virtual bool IsInteractable => true;
    public virtual bool IsHeld => false;
    public virtual bool GetLockOnInteractType(InteractType type) => false;

    protected NetworkVariable<long> nv_lockedBy = new NetworkVariable<long>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool CanNetworkInteract(InteractType type, ulong sender) => nv_lockedBy.Value < 0 || sender == (ulong)nv_lockedBy.Value;

    public virtual void OnHoverEnter()
    {

    }

    public virtual void OnHoverExit()
    {

    }

    public void OnInteract(InteractType type, ulong sender)
    {
        if (!CanNetworkInteract(type, sender))
            return;
        OnInteractServerRPC(type, sender);
    }

    [ServerRpc(RequireOwnership = false)]
    protected virtual void OnInteractServerRPC(InteractType type, ulong sender)
    {
        if (!CanNetworkInteract(type, sender))
            return;
        if (GetLockOnInteractType(type))
            nv_lockedBy.Value = (long)sender;
        ChangeOwnership(sender);
        OnInteractClientRPC(type, sender);
    }
    
    // CHANGE THIS ONCE WE WORK OUT WHY NetworkObject.ChangeOwnership SCREWS WITH THE RIGIDBODY KINEMATIC-NESS
    protected virtual void ChangeOwnership(ulong sender)
    {
        bool isKinematic = true;
        if (this is ContainerBox)
        {
            isKinematic = (this as ContainerBox).Rb.isKinematic;
        }
        else if (this is Product)
        {
            isKinematic = (this as Product).Rb.isKinematic;
        }

        NetworkObject.ChangeOwnership(sender);

        if (this is ContainerBox)
        {
            (this as ContainerBox).Rb.isKinematic = isKinematic;
        }
        else if (this is Product)
        {
            (this as Product).Rb.isKinematic = isKinematic;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    [ClientRpc]
    protected virtual void OnInteractClientRPC(InteractType type, ulong sender)
    {
        if (!CanNetworkInteract(type, sender))
            return;
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

    [ServerRpc(RequireOwnership = false)]
    protected virtual void UnlockServerRPC()
    {
        nv_lockedBy.Value = -1;
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

    public virtual void ToggleComponents(bool isEnabled)
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
