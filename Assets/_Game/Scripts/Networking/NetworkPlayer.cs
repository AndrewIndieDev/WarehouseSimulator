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
        interactable.transform.parent = interactionController.heldItemSlot;
        interactable.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        interactionController.currentHeld = interactable;
    }
}
