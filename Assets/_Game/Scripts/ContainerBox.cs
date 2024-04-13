using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerBox : BaseInteractable
{
    #region Base Interactable
    public override InteractableType Type => InteractableType.Container;
    public override bool IsInteractable => true;
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
        if (sender != NetworkManager.LocalClientId) return;
        if (!isHeld) // If the object we are interacting with is not held
        {
            isHeld = true;
            FreezeContainer();
            NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>().PickupInteractable(this);
        }
        else // If the object we are interacting with is held
        {
            isHeld = false;
            UnFreezeContainer();
            UnlockServerRPC();
        }
        FreezeContents(isHeld);
    }

    protected override void HandleSecondaryInteraction(ulong sender)
    {
        ToggleOpen();
    }

    protected override void HandleHeldInteraction(ulong sender)
    {
        if (sender != NetworkManager.LocalClientId) return;
        if (boxContents.Count > 0)
            return;

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

    void Start()
    {
        SetContainerClosed(false);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("NoInteraction"))
    //        return;
    //    Product product = other.gameObject.GetComponent<Product>();
    //    if (product != null && boxContents.Contains(product))
    //    {
    //        if (isClosing == true)
    //            failedClose = true;
    //    }
    //}

    private Collider[] GetProductsInsideBox()
    {
        overlapCollider.gameObject.SetActive(true);
        Collider[] found = Physics.OverlapBox(overlapCollider.bounds.center, overlapCollider.bounds.extents * 2, overlapCollider.transform.rotation, interactableLayer);
        overlapCollider.gameObject.SetActive(false);
        return found;
    }

    private bool ObjectsStickingOut()
    {
        closeCollider.gameObject.SetActive(true);
        Collider[] found = Physics.OverlapBox(closeCollider.bounds.center, closeCollider.bounds.extents * 2, closeCollider.transform.rotation, interactableLayer);
        return found.Length > 1; // we get > 1 because we also get the box itself
    }

    private void FreezeContents(bool freeze)
    {
        if (freeze)
        {
            Collider[] hits = GetProductsInsideBox();
            foreach (var hit in hits)
            {
                Product product = hit.GetComponent<Product>();
                if (product != null)
                {
                    product.transform.SetParent(boxContentsParent, true);
                    boxContents.Add(product);
                    product.FreezeProduct();
                }
            }
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

    public void OnFlapHit()
    {
        SetContainerClosed(false);
    }

    private void SetContainerClosed(bool isClosing)
    {
        this.isClosing = isClosing;
        if (animationCoroutine == null)
            animationCoroutine = StartCoroutine(OpenAnimation());
        
        if (isClosing)
        {
            if (ObjectsStickingOut())
            {
                Debug.Log("Items found sticking out.");
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
        SetContainerClosed(!isClosing);
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
}
