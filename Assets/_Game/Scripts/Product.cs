using UnityEngine;

public class Product : BaseInteractable
{
    #region Base Interactable
    public override InteractableType Type => InteractableType.Pickup;
    public override bool IsHeld => isHeld;
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
    }

    protected override void HandleHeldInteraction(ulong sender)
    {
        UnFreezeProduct();
        rb.AddForce(Camera.main.transform.forward * 30, ForceMode.Impulse);
        isHeld = false;
    }

    #endregion

    public Texture2D ProductIcon { get { return productIcon; } set { productIcon = value; } }

    [Header("Generic Product References")]
    public string productId;
    [SerializeField] protected bool isHeld = false;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected MeshRenderer mr;
    [SerializeField] protected Texture2D productIcon;

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

    protected virtual void OnDestroy()
    {
        ProductManager.Instance.RemoveProductCount(productId, 1);
    }
}
