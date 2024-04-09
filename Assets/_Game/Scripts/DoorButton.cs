using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour, IInteractable
{
    public DoorController door;
    
    public InteractableType Type => InteractableType.None;
    public bool IsInteractable => door && !door.isInTransition;
    public bool IsHeld => false;

    public void OnHoverEnter()
    {
        
    }

    public void OnHoverExit()
    {
        
    }

    public void OnInteract(InteractType type)
    {
        switch (type)
        {
            case InteractType.Primary:
                if (door == null) return;
                door.ToggleOpen();
                break;
            case InteractType.Secondary:
                break;
            default:
                break;
        }
        
    }
}
