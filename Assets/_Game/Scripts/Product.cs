using UnityEngine;

public class Product : BaseInteractable, IInteractable
{
    public InteractableType Type => InteractableType.Pickup;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => isHeld;
    public Texture2D ProductIcon { get { return productIcon;  } set { productIcon = value; } }

    public string productId;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isHeld = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private new Collider collider;
    [SerializeField] private Texture2D productIcon;

    public void OnInteract(InteractType type)
    {
        switch (type)
        {
            case InteractType.Primary:
                if (!isHeld) // If the object we are interacting with is not held
                {
                    isHeld = true;
                    FreezeProduct();
                }
                else // If the object we are interacting with is held
                {
                    isHeld = false;
                    UnFreezeProduct();
                }
                break;
            case InteractType.Secondary:
                break;
            case InteractType.HeldInteraction:
                UnFreezeProduct();
                rb.AddForce(Camera.main.transform.forward * 30, ForceMode.Impulse);
                isHeld = false;
                break;
            default:
                break;
        }
    }

    public void FreezeProduct()
    {
        rb.isKinematic = true;
        ToggleComponents(false);
    }

    public void UnFreezeProduct()
    {
        rb.isKinematic = false;
        ToggleComponents(true);
    }

    public void OnHoverEnter()
    {
        // glow around object
    }

    public void OnHoverExit()
    {
        // stop glow around object
    }
    
    protected virtual void Start()
    {
        ProductManager.Instance.AddProductCount(productId, 1);
    }

    private void Update()
    {
        if (transform.position.y < -10)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = GameManager.Instance.RespawnPosition;
        }
    }

    private void OnDestroy()
    {
        ProductManager.Instance.RemoveProductCount(productId, 1);
    }
}
