using UnityEngine;

public class Product : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Pickup;
    public Transform Transform => transform;
    public Collider Collider => collider;
    public Rigidbody Rigidbody => rb;
    public Transform Seat => null;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => isHeld;

    public string productId;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isHeld = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private new Collider collider;

    public void OnInteract(InteractType interactType)
    {
        switch (interactType)
        {
            case InteractType.Default:
                if (!isHeld) // If the object we are interacting with is not held
                {
                    isHeld = true;
                    rb.isKinematic = true;
                    Collider.enabled = false;
                }
                else // If the object we are interacting with is held
                {
                    isHeld = false;
                    rb.isKinematic = false;
                    Collider.enabled = true;
                }
                break;
            case InteractType.Place:
                Destroy(Transform.gameObject);
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
