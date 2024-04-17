using UnityEngine;

public class Product : BaseInteractable
{
    #region Base Interactable
    public override InteractableType Type => InteractableType.Pickup;
    public override bool IsHeld => isHeld;
    public override bool GetLockOnInteractType(InteractType type)
    {
        return type switch
        {
            InteractType.Primary => true,
            _ => false
        };
    }
    public override void OnHoverEnter()
    {
        foreach (var mat in mr.materials)
        {
            if (mat.HasProperty("_Scale"))
            {
                mat.SetFloat("_Scale", 1.03f);
            }
        }
    }
    public override void OnHoverExit()
    {
        foreach (var mat in mr.materials)
        {
            if (mat.HasProperty("_Scale"))
            {
                mat.SetFloat("_Scale", 0f);
            }
        }
    }

    protected override void HandlePrimaryInteraction(ulong sender)
    {
        if (sender != NetworkManager.LocalClientId) return;
        if (!isHeld) // If the object we are interacting with is not held
        {
            isHeld = true;
            FreezeProduct();
            NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>().PickupInteractable(this);
        }
        else // If the object we are interacting with is held
        {
            isHeld = false;
            UnFreezeProduct();
        }
    }

    protected override void HandleHeldInteraction(ulong sender)
    {
        if (sender != NetworkManager.LocalClientId) return;
        if (!IsHeld) return;
        UnFreezeProduct();
        rb.AddForce(Camera.main.transform.forward * 30, ForceMode.Impulse);
        isHeld = false;
    }
    #endregion

    public Texture2D ProductIcon { get { return productIcon; } set { productIcon = value; } }
    public bool IsInContainer { get; private set; }
    public ContainerBox ContainedIn { get { return containedIn; } set { containedIn = value; } }

    [Header("Generic Product References")]
    public string productId;
    [SerializeField] protected bool isHeld = false;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected float productMass;
    [SerializeField] protected MeshRenderer mr;
    [SerializeField] protected Texture2D productIcon;

    private ContainerBox containedIn;

    private void Awake()
    {
        productMass = rb.mass;
    }

    public void FreezeProduct()
    {
        rb.isKinematic = true;
        rb.mass = 0;
        IsInContainer = !isHeld;
    }

    public void UnFreezeProduct()
    {
        rb.isKinematic = false;
        ToggleComponents(true);
        rb.mass = productMass;
        IsInContainer = false;
        if (containedIn != null)
        {
            containedIn.RemoveProduct(this);
            containedIn = null;
        }
        UnlockServerRPC();
    }
    
    protected virtual void Start()
    {
        ProductManager.Instance.AddProductCount(productId, 1);
    }

    protected virtual void Update()
    {
        if (transform.position.y < -10)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = GameManager.Instance.RespawnPosition;
        }
    }

    protected virtual new void OnDestroy()
    {
        ProductManager.Instance.RemoveProductCount(productId, 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsInContainer || IsHeld)
            return;

        ContainerBox container = collision.gameObject.GetComponent<ContainerBox>();
        if (container != null)
        {
            if (container.AddExistingProduct(this))
            {
                Debug.Log($"Added {productId} to {container.name}");
                containedIn = container;
            }
        }
    }
}
