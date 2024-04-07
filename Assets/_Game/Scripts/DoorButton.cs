using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour, IInteractable
{
    public DoorController door;
    
    public InteractableType Type => InteractableType.None;
    public Transform Transform => null;
    public Collider Collider => null;
    public Rigidbody Rigidbody => null;
    public Transform Seat => null;
    public bool IsInteractable => door && !door.isInTransition;
    public bool IsHeld => false;
    public List<Component> DisableOnPlacement => null;

    public void OnHoverEnter()
    {
        
    }

    public void OnHoverExit()
    {
        
    }

    public void OnInteract(InteractType interactType)
    {
        if (door == null) return;
        door.ToggleOpen();
    }
}
