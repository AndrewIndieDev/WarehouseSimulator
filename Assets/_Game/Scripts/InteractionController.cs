using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform heldItemSlot;
    [SerializeField] private IInteractable currentHeld;
    [SerializeField] private IInteractable currentHover;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private LayerMask placementLayers;
    [SerializeField] private Material acceptablePlacementMaterial;
    [SerializeField] private Material unacceptablePlacementMaterial;
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Renderer placementRenderer => currentHeld != null ? currentHeld.transform.GetComponentInChildren<Renderer>() : null;

    private bool isPlacing;

    void Update()
    {
        if (currentHover == null || !currentHover.IsHeld)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, 3f, interactionLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable == null)
                    interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null && currentHover != interactable && interactable.IsInteractable)
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

        if (Input.GetButtonDown("Interact") && !isPlacing)
        {
            if (currentHeld != null)
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
                        if ((currentHover as InteractableButton) == null)
                            HandlePickup();
                        else
                            (currentHover as InteractableButton).OnInteract(InteractType.Primary);
                        break;
                }
            }
        }

        if (Input.GetButtonDown("InteractOther"))
        {
            if (!Input.GetButton("Placement"))
            {
                if (currentHover != null)
                {
                    currentHover.OnInteract(InteractType.Secondary);
                }

                if (currentHeld != null)
                {
                    currentHeld.OnInteract(InteractType.HeldInteraction);
                }
            }
        }

        if (Input.GetButton("Placement"))
        {
            if (currentHeld == null)
                return;

            isPlacing = true;

            if (originalMaterial == null)
                originalMaterial = placementRenderer.material;

            bool acceptable = false;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, 5f, placementLayers))
            {
                currentHeld.transform.parent = null;
                currentHeld.transform.SetPositionAndRotation(hit.point, Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f));
                
                placementRenderer.material = acceptablePlacementMaterial;

                acceptable = true;

                ToggleComponents((currentHeld as BaseInteractable).disableOnGhostPlacement, false);
            }
            else
            {
                currentHeld.transform.parent = heldItemSlot;
                currentHeld.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                
                placementRenderer.material = originalMaterial;

                acceptable = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!acceptable)
                    return;
                currentHeld.OnInteract(InteractType.Primary);
                currentHeld.transform.parent = null;
                currentHeld.transform.SetPositionAndRotation(hit.point, Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f));
                placementRenderer.material = originalMaterial;
                originalMaterial = null;
                ToggleComponents((currentHeld as BaseInteractable).disableOnGhostPlacement, true);
                currentHeld = null;
            }
        }
        else
        {
            isPlacing = false;

            if (currentHeld == null)
                return;

            currentHeld.transform.parent = heldItemSlot;
            currentHeld.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (originalMaterial != null)
            {
                placementRenderer.material = originalMaterial;
                originalMaterial = null;
                ToggleComponents((currentHeld as BaseInteractable).disableOnGhostPlacement, true);
            }
        }

        heldItemSlot.localRotation = Quaternion.Euler(-MovementController.Instance.CurrentPitch, 0f, 0f);
    }

    private void HandlePickup()
    {
        if (currentHover != null && currentHover.Type == InteractableType.None)
        {
            currentHover.OnInteract(InteractType.Primary);
            currentHover = null;
            return;
        }
        if (currentHeld == null)
        {
            currentHover.OnInteract(InteractType.Primary);
            currentHover.transform.parent = heldItemSlot;
            currentHover.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            //currentHover.Collider.enabled = false;
            currentHeld = currentHover;
            currentHover = null;
        }
        else
        {
            currentHeld.OnInteract(InteractType.Primary);
            currentHeld.transform.parent = null;
            //currentHeld.Transform.SetPositionAndRotation(cam.transform.position + cam.transform.forward * 0.75f, Quaternion.identity);
            //currentHeld.Collider.enabled = true;
            currentHeld = null;
        }
    }

    private void HandleVehicle()
    {
        // TODO
    }

    private void ToggleComponents(List<Component> components, bool enabled)
    {
        if (components == null)
            return;
        foreach (Component component in components)
        {
            MeshRenderer mr = (component as MeshRenderer);
            Collider col = (component as Collider);
            if (mr != null)
                mr.enabled = enabled;
            else if (col != null)
                col.enabled = enabled;
        }
    }
}
