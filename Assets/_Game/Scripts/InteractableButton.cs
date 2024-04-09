using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.None;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => false;

    [SerializeField] private bool isInteractable = true;
    [SerializeField] private UnityAction onInteract;

    public void OnInteract(InteractType type)
    {
        switch (type)
        {
            case InteractType.Primary:
                onInteract?.Invoke();
                break;
            case InteractType.Secondary:
                break;
            default:
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
