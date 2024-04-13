using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PersistentClient : NetworkBehaviour
{
    public static PersistentClient LocalInstance { get; private set; }
    public static Dictionary<ulong, PersistentClient> AllInstances { get; private set; }
    public PlayerObject PlayerObject { get { return playerObject; } }

    [SerializeField] private bool debugMessages;
    [SerializeField] private GameObject playerObjectPrefab;
    [SerializeField] private PlayerObject playerObject;

    #region Network Variables
    private void SetupNetworkVariables(bool onDestroy = false)
    {
        if (!onDestroy)
        {
            nv_DisplayName.OnValueChanged += OnPlayerNameChanged;
        }
        else
        {
            nv_DisplayName.OnValueChanged -= OnPlayerNameChanged;
        }
    }
    private NetworkVariable<FixedString32Bytes> nv_DisplayName = new NetworkVariable<FixedString32Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public string PlayerName { get { return nv_DisplayName.Value.ToString(); } }
    private void OnPlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {

    }
    #endregion

    #region Network Spawn and Despawn
    public override void OnNetworkSpawn()
    {
        SetupNetworkVariables();

        gameObject.name = $"PersistentClient <Player {OwnerClientId}>";

        if (IsOwner)
        {
            DebugMessage("PersistentClient Initialized. . .");
            nv_DisplayName.Value = $"Player {OwnerClientId}";
            LocalInstance = this;
            if (AllInstances == null)
                AllInstances = new();
            AllInstances.Add(OwnerClientId, this);
        }
    }
    public override void OnNetworkDespawn()
    {
        DebugMessage("OnNetworkDespawn. . .");
        SetupNetworkVariables(onDestroy: true);
    }
    #endregion

    #region Delegates
    private void OnGameStarted()
    {
        DebugMessage("OnGameStarted. . .");
        if (IsServer)
        {
            if (playerObject == null)
            {
                DebugMessage("Spawning a PlayerObject. . .");
                playerObject = Instantiate(playerObjectPrefab).GetComponent<PlayerObject>();
                playerObject.transform.position = new Vector3(0, 0, 0);
                playerObject.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
            }
        }
    }
    #endregion

    #region Public Methods
    public void SetPlayerObject(PlayerObject playerObject)
    {
        DebugMessage("SetPlayerObject. . .");
        if (IsOwner)
        {
            this.playerObject = playerObject;
        }
    }
    #endregion

    private void DebugMessage(string message)
    {
        if (debugMessages)
        {
            Debug.Log($"PERSISTENTCLIENT <{OwnerClientId}> :: {message}");
        }
    }
}