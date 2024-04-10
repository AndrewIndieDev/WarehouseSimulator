using System.Collections;
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
    public override void HandlePrimaryInteraction()
    {
        if (!isHeld) // If the object we are interacting with is not held
        {
            isHeld = true;
            FreezeContainer();
        }
        else // If the object we are interacting with is held
        {
            isHeld = false;
            UnFreezeContainer();
        }
    }
    public override void HandleSecondaryInteraction()
    {
        ToggleOpen();
    }
    public override void HandleHeldInteraction()
    {
        OnInteract(InteractType.Secondary);
    }
    #endregion

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private AnimationCurve failedCloseCurve;
    [SerializeField] private SkinnedMeshRenderer mr;
    private bool isHeld;
    private float animationTime;
    private bool isClosing;
    private bool failedClose;
    Coroutine animationCoroutine;

    void Start()
    {
        SetContainerClosed(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.contacts[0].thisCollider.transform == transform) return;
        if (other.gameObject.GetComponent<Product>() != null)
        {
            if (isClosing == true)
                failedClose = true;
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
