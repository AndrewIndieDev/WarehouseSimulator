using UnityEngine;

public enum InteractableType
{
    None,
    Container,
    Pickup,
    Vehicle,
    Button
}

public enum InteractType
{
    None,
    Primary,
    Secondary,
    HeldInteraction
}

public interface IInteractable
{
    public InteractableType Type { get; }
    public bool IsInteractable { get; }
    public bool IsHeld { get; }
    public void OnHoverEnter();
    public void OnHoverExit();
    public void OnInteract(InteractType type, ulong sender);
    public Transform transform { get; }
    public GameObject gameObject { get; }
}
