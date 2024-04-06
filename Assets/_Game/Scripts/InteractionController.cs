using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform heldItemSlot;
    [SerializeField] private IInteractable currentHeld;
    [SerializeField] private IInteractable currentHover;
    [SerializeField] private LayerMask interactionLayer;

    void Update()
    {
        if (currentHover == null || !currentHover.IsHeld)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, 2f, interactionLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null && currentHover != interactable)
                {
                    currentHover?.OnHoverExit();
                    interactable.OnHoverEnter();
                    currentHover = interactable;
                }
            }
            else
            {
                currentHover?.OnHoverExit();
                currentHover = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentHeld != null && currentHeld.Type == InteractableType.Pickup &&
                currentHover != null && currentHover.Type == InteractableType.Container)
            {
                if ((currentHover as Box).PlaceItemInContainer(currentHeld))
                {
                    currentHeld.OnInteract(InteractType.Place);
                    currentHeld = null;
                }
            }
            else if (currentHeld != null)
            {
                HandlePickup();
            }
            else if (currentHover != null)
            {
                switch (currentHover.Type)
                {
                    case InteractableType.Vehicle:
                        HandleVehicle();
                        break;
                    default:
                        HandlePickup();
                        break;
                }
            }
        }
    }

    private void HandlePickup()
    {
        if (currentHeld == null)
        {
            currentHover.OnInteract(InteractType.Default);
            currentHover.Transform.parent = heldItemSlot;
            currentHover.Transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            currentHover.Collider.enabled = false;
            currentHeld = currentHover;
            currentHover = null;
        }
        else
        {
            currentHeld.OnInteract(InteractType.Default);
            currentHeld.Transform.parent = null;
            currentHeld.Transform.SetPositionAndRotation(cam.transform.position + cam.transform.forward * 0.75f, Quaternion.identity);
            currentHeld.Collider.enabled = true;
            currentHeld = null;
        }
    }

    private void HandleVehicle()
    {
        // TODO
    }
}
