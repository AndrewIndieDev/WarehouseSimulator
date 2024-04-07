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
    [SerializeField] private MeshRenderer placementRenderer => currentHeld != null ? currentHeld.Transform.GetComponentInChildren<MeshRenderer>() : null;

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
                if ((currentHover as Container).PlaceItemInContainer(currentHeld))
                {
                    currentHeld.OnInteract(InteractType.PlaceInContainer);
                    currentHeld = null;
                }
            }
            else if (currentHeld != null && currentHover == null)
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
                        if ((currentHover as TakeOutButton) == null)
                            HandlePickup();
                        else
                        {
                            TakeOutButton takeOut = currentHover as TakeOutButton;
                            if (currentHeld == null)
                            {
                                currentHover.OnInteract(InteractType.TakeOutOfContainer);
                                IInteractable item = takeOut.Container.TakeItemOutOfContainer();
                                if (item != null)
                                {
                                    item.OnInteract(InteractType.Default);
                                    item.Transform.parent = heldItemSlot;
                                    item.Transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                                    item.Collider.enabled = false;
                                    currentHover = null;
                                    currentHeld = item;
                                }
                            }
                            else
                            {
                                if (takeOut.Container.PlaceItemInContainer(currentHeld))
                                {
                                    currentHeld.OnInteract(InteractType.PlaceInContainer);
                                    currentHeld = null;
                                }
                            }
                        }
                        break;
                }
            }
        }

        if (Input.GetMouseButton(1)) // Right click held
        {
            if (currentHeld == null)
                return;
            
            if (originalMaterial == null)
                originalMaterial = placementRenderer.material;

            bool acceptable = false;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, 5f, placementLayers))
            {
                currentHeld.Transform.parent = null;
                currentHeld.Transform.SetPositionAndRotation(hit.point, Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f));
                
                placementRenderer.material = acceptablePlacementMaterial;

                acceptable = true;

                ToggleComponents(currentHeld.DisableOnPlacement, false);
            }
            else
            {
                currentHeld.Transform.parent = heldItemSlot;
                currentHeld.Transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                
                placementRenderer.material = originalMaterial;

                acceptable = false;

                ToggleComponents(currentHeld.DisableOnPlacement, true);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!acceptable)
                    return;
                currentHeld.OnInteract(InteractType.Default);
                currentHeld.Transform.parent = null;
                currentHeld.Transform.SetPositionAndRotation(hit.point, Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f));
                placementRenderer.material = originalMaterial;
                originalMaterial = null;
                ToggleComponents(currentHeld.DisableOnPlacement, true);
                currentHeld = null;
            }
        }
        else
        {
            if (currentHeld == null)
                return;

            currentHeld.Transform.parent = heldItemSlot;
            currentHeld.Transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (originalMaterial != null)
            {
                placementRenderer.material = originalMaterial;
                originalMaterial = null;
                ToggleComponents(currentHeld.DisableOnPlacement, true);
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
            //currentHover.Collider.enabled = false;
            currentHeld = currentHover;
            currentHover = null;
        }
        else
        {
            currentHeld.OnInteract(InteractType.Default);
            currentHeld.Transform.parent = null;
            currentHeld.Transform.SetPositionAndRotation(cam.transform.position + cam.transform.forward * 0.75f, Quaternion.identity);
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
