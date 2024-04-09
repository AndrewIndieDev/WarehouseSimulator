using System.Collections;
using UnityEngine;

public class ContainerBox : BaseInteractable, IInteractable
{
    public InteractableType Type => InteractableType.Container;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => isHeld;

    [SerializeField] private bool isInteractable;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    private bool isHeld;
    private float animationTime;
    private bool isClosing;
    Coroutine animationCoroutine;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.gameObject.name} entered.");
    }

    private IEnumerator OpenAnimation()
    {
        while (true)
        {
            if (isClosing)
            {
                animationTime -= Time.deltaTime;
                if (animationTime <= 0)
                    break;
            }
            else
            {
                animationTime += Time.deltaTime;
                if (animationTime >= 1)
                    break;
            }

            animator.SetFloat("OpenTime", animationTime);

            yield return null;
        }
        animationCoroutine = null;
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

    public void OnHoverEnter()
    {
        
    }

    public void OnHoverExit()
    {
        
    }

    public void OnInteract(InteractType type)
    {
        switch (type)
        {
            case InteractType.Primary:
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
                break;
            case InteractType.Secondary:
                ToggleOpen();
                break;
            case InteractType.HeldInteraction:
                OnInteract(InteractType.Secondary);
                break;
            default:
                break;
        }
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
