using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseInteractable : MonoBehaviour
{
    public List<Component> disableOnGhostPlacement = new();

    public void ToggleComponents(bool enabled)
    {
        foreach (Component component in disableOnGhostPlacement)
        {
            MeshRenderer mr = (component as MeshRenderer);
            Collider col = (component as Collider);
            if (mr != null)
                mr.enabled = enabled;
            else if (col != null)
                col.enabled = enabled;
        }
    }
}
