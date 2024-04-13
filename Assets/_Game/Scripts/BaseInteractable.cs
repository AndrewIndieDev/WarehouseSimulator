using System.Collections.Generic;
using System.Xml.Serialization;
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
        NetworkObject.ChangeOwnership(sender);
        OnInteractClientRPC(type, sender);
    }
    
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
