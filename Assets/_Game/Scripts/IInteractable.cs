using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    None,
    Container,
    Pickup,
    Vehicle
}

public enum InteractType
{
    Default,
    PlaceInContainer,
    TakeOutOfContainer
}

public interface IInteractable
{
    public InteractableType Type { get; }
    public Transform Transform { get; }
    public Collider Collider { get; }
    public Rigidbody Rigidbody { get; }
    public Transform Seat { get; }
    public bool IsInteractable { get; }
    public bool IsHeld { get; }
    public void OnHoverEnter();
    public void OnHoverExit();
    public void OnInteract(InteractType interactType);
    public List<Behaviour> DisableOnPlacement { get; }
}
