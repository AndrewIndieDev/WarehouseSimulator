using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : BaseInteractable
{
    #region Base Interactable
    public override InteractableType Type => InteractableType.Button;
    public override bool IsInteractable => true;
    public override bool IsHeld => false;
    public override void HandlePrimaryInteraction()
    {
        onInteract?.Invoke();
    }
    public override void HandleSecondaryInteraction()
    {
        HandlePrimaryInteraction();
    }
    #endregion

    [SerializeField] private bool isInteractable = true;
    [SerializeField] private UnityEvent onInteract;
}
