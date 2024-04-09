using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ContainerBox : BaseInteractable, IInteractable
{
    public InteractableType Type => InteractableType.Container;
    public bool IsInteractable => isInteractable;
    public bool IsHeld => isHeld;

    [SerializeField] private bool isInteractable;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private List<BoxCollider> flapColliders = new();
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private AnimationCurve failedCloseCurve;
    private bool isHeld;
    private float animationTime;
    private bool isClosing;
    private bool failedClose;
    Coroutine animationCoroutine;

    void Start()
    {
        SetContainerClosed(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Product>() != null)
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
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
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

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    foreach (var collider in flapColliders)
    //    {
    //        Gizmos.matrix = Matrix4x4.TRS(collider.bounds.center, collider.transform.rotation, collider.size * collider.transform.lossyScale.x);
    //        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    //    }
    //}
}
