using UnityEngine;

public class TakeOutButton : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.None;
    public Transform Transform => transform;
    public Collider Collider => null;
    public Rigidbody Rigidbody => null;
    public Transform Seat => null;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => false;

    public Container Container => container;

    [SerializeField] private bool isInteractable = true;
    [SerializeField] private Container container;

    public void OnInteract(InteractType interactType)
    {
        switch (interactType)
        {
            case InteractType.TakeOutOfContainer:

                break;
            default:
                Debug.LogError("Unhandled InteractType: " + interactType);
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
}
