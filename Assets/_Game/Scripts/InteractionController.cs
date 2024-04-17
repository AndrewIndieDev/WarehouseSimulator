using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public IInteractable currentHeld;
    public IInteractable currentHover;
    public Transform heldItemSlot;

    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private LayerMask placementLayers;
    [SerializeField] private Material acceptablePlacementMaterial;
    [SerializeField] private Material unacceptablePlacementMaterial;
    [SerializeField] private Material originalMaterial;

    [Header("Variables")]
    [SerializeField] private float interactionRange = 3f;

    private Renderer placementRenderer => currentHeld != null ? currentHeld.transform.GetComponentInChildren<Renderer>() : null;
    private bool isPlacing;
    
    private ulong ownerID;

    public void Init()
    {
        ownerID = GetComponent<NetworkObject>().OwnerClientId;
    }
    
    void Update()
    {
        ValidateCurrentHeld();
        HandleCurrentHeldRotation();
        HandleHoverRaycast();
        HandleInteractButton();
        HandleInteractOtherButton();
        HandlePlacementButton();
    }

    private void ValidateCurrentHeld()
    {
        if (currentHeld == null || currentHeld.IsHeld)
            return;

        if (currentHeld.transform.parent == heldItemSlot)
            currentHeld.transform.parent = null;
        currentHeld = null;
    }

    private void HandleCurrentHeldRotation()
    {
        if (currentHeld == null)
            return;

        heldItemSlot.localRotation = Quaternion.Euler(-MovementController.Instance.CurrentPitch, 0f, 0f);
    }

    private void HandleHoverRaycast()
    {
        IInteractable interactable = null;
        RaycastHit[] found = Physics.RaycastAll(cam.transform.position, cam.transform.forward, interactionRange, interactionLayer);
        Vector3 closestPoint = Vector3.one * interactionRange * 2;
        for (int i = 0; i < found.Length; i++)
        {
            IInteractable previous = interactable;

            interactable = found[i].collider.GetComponent<IInteractable>();

            if (interactable == null)
            {
                interactable = found[i].collider.GetComponentInParent<IInteractable>();
            }
            
            if (interactable != null)
            {
                if (Vector3.Distance(found[i].point, transform.position) < Vector3.Distance(closestPoint, transform.position))
                {
                    closestPoint = found[i].point;
                }
                else
                {
                    interactable = previous;
                }
            }
        }

        if (interactable == null)
        {
            currentHover?.OnHoverExit();
            currentHover = null;
        }

        if (interactable != null && interactable.IsInteractable)
        {
            currentHover?.OnHoverExit();
            interactable.OnHoverEnter();
            currentHover = interactable;
        }
    }

    private void HandleInteractButton()
    {
        if (Input.GetButtonDown("Interact") && !isPlacing)
        {
            if (currentHeld != null)
            {
                currentHeld.OnInteract(InteractType.Primary, ownerID);
                currentHeld.transform.parent = null;
            }
            else if (currentHover != null)
            {
                switch (currentHover.Type)
                {
                    case InteractableType.Vehicle:
                        //HandleVehicle();
                        break;
                    default:
                        if ((currentHover as InteractableButton) == null)
                        {
                            currentHover.OnInteract(InteractType.Primary, ownerID);
                            //currentHover.transform.parent = heldItemSlot;
                            //currentHover.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                            //currentHeld = currentHover;
                        }
                        else
                            (currentHover as InteractableButton).OnInteract(InteractType.Primary, ownerID);
                        break;
                }
            }
        }
    }

    private void HandleInteractOtherButton()
    {
        if (Input.GetButtonDown("InteractOther"))
        {
            if (!Input.GetButton("Placement"))
            {
                if (currentHeld != null)
                {
                    currentHeld.OnInteract(InteractType.HeldInteraction, ownerID);
                }
                else if (currentHover != null)
                {
                    currentHover.OnInteract(InteractType.Secondary, ownerID);
                }
            }
        }
    }

    private void HandlePlacementButton()
    {
        if (Input.GetButton("Placement"))
        {
            if (currentHeld == null)
                return;

            isPlacing = true;

            if (originalMaterial == null)
            {
                originalMaterial = placementRenderer.material;
            }

            bool acceptable = false;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, 5f, placementLayers))
            {
                currentHeld.transform.parent = null;
                currentHeld.transform.SetPositionAndRotation(hit.point, Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f));

                placementRenderer.material = acceptablePlacementMaterial;

                acceptable = true;

                (currentHeld as BaseInteractable).ToggleComponents(false);
                //ToggleComponents((currentHeld as BaseInteractable).disableOnGhostPlacement, false);
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
                currentHeld.OnInteract(InteractType.Primary, ownerID);
                currentHeld.transform.parent = null;
                currentHeld.transform.SetPositionAndRotation(hit.point, Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f));
                placementRenderer.material = originalMaterial;
                originalMaterial = null;
                (currentHeld as BaseInteractable).ToggleComponents(true);
                //ToggleComponents((currentHeld as BaseInteractable).disableOnGhostPlacement, true);
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
            }
        }
    }

    //private void ToggleComponents(List<Component> components, bool enabled)
    //{
    //    if (components == null)
    //        return;
    //    foreach (Component component in components)
    //    {
    //        MeshRenderer mr = (component as MeshRenderer);
    //        Collider col = (component as Collider);
    //        if (mr != null)
    //            mr.enabled = enabled;
    //        else if (col != null)
    //            col.enabled = enabled;
    //    }
    //}
}
