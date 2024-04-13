using System;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class DebugServerManager : MonoBehaviour
{
#if UNITY_EDITOR
    public bool startHostOnDavid = true;
    private ulong davidID = 76561198045888021;
    private ulong andrewID = 76561198173399099;
    
    private void Start()
    {
        if (SteamClient.SteamId == davidID && startHostOnDavid || SteamClient.SteamId == andrewID && !startHostOnDavid)
            GetComponent<NetworkManager>().StartHost();
        else
        {
            GetComponent<SteamP2PRelayTransport>().serverId = SteamClient.SteamId == davidID ? andrewID : davidID;
            GetComponent<NetworkManager>().StartClient();
        }
    }
#endif
}
