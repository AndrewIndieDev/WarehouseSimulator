using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : BaseInteractable
{
    #region Base Interactable
    public override InteractableType Type => InteractableType.Button;
    public override bool IsInteractable => true;
    public override bool IsHeld => false;

    protected override void HandlePrimaryInteraction(ulong sender)
    {
        onInteract?.Invoke();
    }

    protected override void HandleSecondaryInteraction(ulong sender)
    {
        HandlePrimaryInteraction(0);
    }
    #endregion

    [SerializeField] private bool isInteractable = true;
    [SerializeField] private UnityEvent onInteract;
}
