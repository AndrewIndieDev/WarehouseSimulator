using Unity.Netcode;
using UnityEngine;

public class PlayerObject : NetworkBehaviour
{
    [SerializeField] private bool debugMessages;

    #region Virtual Methods
    protected virtual void Start()
    {
        DebugMessage($"{(IsServer ? "Server" : (IsHost ? "Host" : "Client"))}: " + NetworkManager.Singleton.LocalClientId);
        SetAsLocalPlayerObject();
    }
    protected virtual void SetAsLocalPlayerObject()
    {
        if (IsOwner)
        {
            PersistentClient.LocalInstance.SetPlayerObject(this);
        }
    }
    #endregion

    #region Public Methods
    
    #endregion

    protected void DebugMessage(string message)
    {
        if (debugMessages)
        {
            Debug.Log($"PLAYEROBJECT :: {message}");
        }
    }
}
