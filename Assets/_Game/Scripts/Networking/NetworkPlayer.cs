using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    InteractionController interactionController;
    
    public override void OnNetworkSpawn()
    {
        interactionController = GetComponent<InteractionController>();
        interactionController?.Init();
    }

    public void PickupInteractable(IInteractable interactable)
    {
        interactionController.currentHover.transform.parent = interactionController.heldItemSlot;
        interactionController.currentHover.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        interactionController.currentHeld = interactable;
    }
}
