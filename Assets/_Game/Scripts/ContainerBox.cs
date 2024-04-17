using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ContainerBox : BaseInteractable
{
    #region Base Interactable
    public override InteractableType Type => InteractableType.Container;
    public override bool IsInteractable => true;
    public override bool IsHeld => isHeld;
    
    NetworkVariable<bool> isClosed = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
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
        isHeld = !isHeld;
        //FreezeContents(isHeld);
        if (isHeld) // If the object we are interacting with is not held
        {
            FreezeContainer();
            NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>().PickupInteractable(this);
        }
        else // If the object we are interacting with is held
        {
            UnFreezeContainer();
            UnlockServerRPC();
        }
    }

    protected override void HandleSecondaryInteraction(ulong sender)
    {
        if (sender != NetworkManager.LocalClientId) return;
            ToggleOpen();
    }

    protected override void HandleHeldInteraction(ulong sender)
    {
        if (sender != NetworkManager.LocalClientId) return;
        if (!isHeld) return;

        OnInteract(InteractType.Secondary, sender);
    }
    #endregion

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private AnimationCurve failedCloseCurve;
    [SerializeField] private SkinnedMeshRenderer mr;
    [SerializeField] private BoxCollider overlapCollider;
    [SerializeField] private BoxCollider closeCollider;
    [SerializeField] private Transform boxContentsParent;
    private bool isHeld;
    private float animationTime;
    private bool isClosing;
    private bool failedClose;
    Coroutine animationCoroutine;
    private List<Product> boxContents = new();

    public override void OnNetworkSpawn()
    {
        SetContainerClosed(isClosed.Value, false);
    }

    /// <summary>
    /// Adds a product to the container.
    /// </summary>
    /// <param name="productId">string ID of the product to add.</param>
    /// <returns>returns if this product was able to be added.</returns>
    public bool SpawnAndAddProduct(string productId)
    {
        int currentCount = boxContents.Count;
        if (currentCount >= boxContentsParent.childCount)
            return false;
        Product p = ProductManager.Instance.SpawnProductPrefab(productId, boxContentsParent.GetChild(currentCount).position, Quaternion.identity);
        p.transform.SetParent(boxContentsParent.GetChild(currentCount));
        p.transform.localPosition = Vector3.zero;
        p.FreezeProduct();
        p.ContainedIn = this;
        boxContents.Add(p);
        return true;
    }

    public bool AddExistingProduct(Product product)
    {
        int currentCount = boxContents.Count;
        if (currentCount >= boxContentsParent.childCount)
            return false;
        for (int i = 0; i < boxContentsParent.childCount; i++)
        {
            if (boxContentsParent.GetChild(i).childCount == 0)
            {
                product.transform.SetParent(boxContentsParent.GetChild(i));
                break;
            }
        }
        product.transform.localPosition = Vector3.zero;
        product.FreezeProduct();
        boxContents.Add(product);
        return true;
    }

    public void RemoveProduct(Product product)
    {
        if (boxContents.Contains(product))
            boxContents.Remove(product);
    }

    private List<Product> GetProductsInsideBox()
    {
        overlapCollider.gameObject.SetActive(true);
        Collider[] found = Physics.OverlapBox(overlapCollider.bounds.center, overlapCollider.bounds.extents / 2, overlapCollider.transform.rotation, interactableLayer); // Ln 119
        overlapCollider.gameObject.SetActive(false);
        List<Product> products = new();
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i].GetComponent<Product>() != null)
            {
                products.Add(found[i].GetComponent<Product>());
            }
        }
        return products;
    }

    private bool ObjectsStickingOut()
    {
        closeCollider.gameObject.SetActive(true);
        Collider[] found = Physics.OverlapBox(closeCollider.bounds.center, closeCollider.bounds.extents / 2, closeCollider.transform.rotation, interactableLayer);
        return found.Length > 1; // we get > 1 because we also collide with the box itself... wait we shouldn't get this, so the bounds.extents must be too big... maybe try divide by 2? (includes Ln 119)...
    }

    private void FreezeContents(bool freeze)
    {
        // We shouldn't need to call this anymore due to objects being parented to the container, and always frozen in the container.
        if (freeze)
        {
            for (int i = 0; i < boxContents.Count; i++)
            {
                Product product = boxContents[i];
                if (boxContents.Count < boxContentsParent.childCount)
                {
                    product.transform.SetParent(boxContentsParent.GetChild(i));
                    product.transform.localPosition = Vector3.zero;
                    product.transform.localRotation = Quaternion.identity;
                    product.FreezeProduct();
                    boxContents.Add(product);
                }
                else
                {
                    Debug.LogError($"Somehow there are too many objects inside the box? [Count: {boxContents.Count}] [Available: {boxContentsParent.childCount}]");
                }
            }

            //if (boxContents.Count > 0)
            //{
            //    foreach (var product in boxContents)
            //    {
            //        product.transform.parent = null;
            //        product.UnFreezeProduct();
            //    }
            //    boxContents.Clear();
            //}

            //List<Product> hits = GetProductsInsideBox();
            //for (int i = 0; i < hits.Count; i++)
            //{
            //    Product product = hits[i];
            //    if (boxContents.Count < boxContentsParent.childCount)
            //    {
            //        product.transform.SetParent(boxContentsParent.GetChild(i));
            //        product.transform.localPosition = Vector3.zero;
            //        product.transform.localRotation = Quaternion.identity;
            //        product.FreezeProduct();
            //        boxContents.Add(product);
            //    }
            //    else
            //    {
            //        Debug.LogError($"Somehow there are too many objects inside the box? [Count: {boxContents.Count}] [Available: {boxContentsParent.childCount}]");
            //    }
            //}
        }
        else
        {
            foreach (var product in boxContents)
            {
                product.transform.parent = null;
                product.UnFreezeProduct();
            }
            boxContents.Clear();
        }
    }

    private IEnumerator OpenAnimation()
    {
        while (true)
        {
            if (isClosing && !failedClose)
            {
                animationTime -= Time.deltaTime;
                if (animationTime <= 0)
                    break;
            }
            else if (!failedClose)
            {
                animationTime += Time.deltaTime;
                if (animationTime >= 1)
                    break;
            }
            else
            {
                yield return FailedClose();
            }

            animator.SetFloat("OpenTime", animationTime);

            yield return null;
        }

        animationCoroutine = null;
    }

    private IEnumerator FailedClose()
    {
        float animTime = animationTime;
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime;
            animationTime = animTime + failedCloseCurve.Evaluate(i);
            animator.SetFloat("OpenTime", animationTime);
            yield return null;
        }

        failedClose = false;
        isClosing = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetContainerClosedServerRPC(bool isClosing)
    {
        bool failed = false;
        if (isClosing)
        {
            if (ObjectsStickingOut())
            {
                Debug.Log("Items found sticking out.");
                failed = true;
            }
        }

        isClosed.Value = isClosing && !failed;
        SetContainerClosedClientRPC(isClosing, failed);
    }

    [ClientRpc]
    private void SetContainerClosedClientRPC(bool isClosing, bool failedClose)
    {
        SetContainerClosed(isClosing, failedClose);
    }

    private void SetContainerClosed(bool isClosing, bool failedClose)
    {
        this.isClosing = isClosing;
        if (animationCoroutine == null)
            animationCoroutine = StartCoroutine(OpenAnimation());
        if (isClosing)
        {
            if (failedClose)
            {
                closeCollider.gameObject.SetActive(false);
                Invoke(nameof(TempFailedDelay), 0.3f);
            }
        }
        else
        {
            closeCollider.gameObject.SetActive(false);
        }
    }
    
    private void TempFailedDelay()
    {
        failedClose = true;
    }

    private void ToggleOpen()
    {
        SetContainerClosedServerRPC(!isClosing);
    }

    public void FreezeContainer()
    {
        rb.isKinematic = true;
        ToggleComponents(false);
    }

    public void UnFreezeContainer()
    {
        rb.isKinematic = false;
        ToggleComponents(true);
    }

    public override void ToggleComponents(bool isEnabled)
    {
        foreach (Product product in boxContents)
        {
            product.ToggleComponents(isEnabled);
        }
        base.ToggleComponents(isEnabled);
    }
}
