using Unity.Netcode;
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
            RemoveFromContainer();
            FreezeProduct();
            NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>().PickupInteractable(this);
        }
        else // If the object we are interacting with is held
        {
            isHeld = false;
            UnFreezeProduct();
            ReleaseOwnershipServerRPC();
        }
    }

    protected override void HandleHeldInteraction(ulong sender)
    {
        if (sender != NetworkManager.LocalClientId) return;
        if (!IsHeld) return;
        isHeld = false;
        UnFreezeProduct();
        ReleaseOwnershipServerRPC(Camera.main.transform.forward * 7);
    }
    #endregion

    public Texture2D ProductIcon { get { return productIcon; } set { productIcon = value; } }
    public bool IsInContainer { get { return containedIn != null; } }
    public ContainerBox ContainedIn { get { return containedIn; } set { containedIn = value; } }
    public Rigidbody Rb { get { return rb; } }

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

    [ServerRpc]
    private void ReleaseOwnershipServerRPC(Vector3 force = default)
    {
        ChangeOwnership(0);
        UnFreezeProduct();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void FreezeProduct()
    {
        rb.isKinematic = true;
        ToggleComponents(false);
        rb.mass = 0;
    }

    public void UnFreezeProduct()
    {
        rb.isKinematic = false;
        ToggleComponents(true);
        rb.mass = productMass;
        UnlockServerRPC();
    }

    public virtual void PutInContainer(ContainerBox container)
    {
        containedIn = container;
        rb.isKinematic = true;
        rb.mass = 0;
    }

    public virtual void RemoveFromContainer()
    {
        if (containedIn != null)
        {
            containedIn.RemoveProduct(this);
            containedIn = null;
        }
    }
    
    protected virtual void Start()
    {
        ProductManager.Instance.AddProductCount(productId, 1);
    }

    protected virtual void Update()
    {
        if (!IsOwner)
            return;

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
        if (container != null && container.IsOpen)
        {
            if (container.AddExistingProduct(this))
            {
                Debug.Log($"Added {productId} to {container.name}");
                containedIn = container;
            }
        }
    }
}
