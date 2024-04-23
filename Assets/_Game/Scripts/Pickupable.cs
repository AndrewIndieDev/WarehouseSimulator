using UnityEngine;

public class Pickupable : BaseInteractable
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
        
    }
    public override void OnHoverExit()
    {
        
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

    public Rigidbody Rb { get { return rb; } }

    [Header("Generic Product References")]
    [SerializeField] protected bool isHeld = false;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected MeshRenderer mr;

    public void FreezeProduct()
    {
        rb.isKinematic = true;
        ToggleComponents(false);
    }

    public void UnFreezeProduct()
    {
        rb.isKinematic = false;
        ToggleComponents(true);
        UnlockServerRPC();
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
}
