using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkBehaviour))]
public class NetworkComponentManager : MonoBehaviour
{
    public Component[] componentsToEnableWhenMine;
    public Component[] componentsToDisableWhenMine;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool isMine = GetComponent<NetworkObject>().IsOwner;
        foreach (var component in componentsToEnableWhenMine)
        {
            Behaviour behaviour = component as Behaviour;
            if (behaviour != null)
                behaviour.enabled = isMine;
            Collider coll = component as Collider;
            if (coll != null)
                coll.enabled = isMine;
        }
        foreach (var component in componentsToDisableWhenMine)
        {
            Behaviour behaviour = component as Behaviour;
            if (behaviour != null)
                behaviour.enabled = !isMine;
            Collider coll = component as Collider;
            if (coll != null)
                coll.enabled = !isMine;
        }
    }
}
