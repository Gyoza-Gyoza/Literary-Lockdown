using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerClientController : NetworkBehaviour
{
    [Header("Player Stats")]
    private NetworkVariable<FixedString512Bytes> playerName = new NetworkVariable<FixedString512Bytes>("Player Connected", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public string m_PlayerName;

    [Header("Tower Stats")]
    public int maxTowers;
    public NetworkVariable<int> currentTowers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkObject towerPrefab;

    [Header("UI")]
    public GameObject towerSpawningUI;

    public void Awake()
    {
        // Get the UI GameObject reference
        towerSpawningUI = UIManager.Instance.TowerSpawner;
    }

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += OnPlayerNameChanged;


        if (IsOwner)
        {
            ChangeGameObjectNameRpc($"Player_{NetworkManager.Singleton.LocalClientId.ToString()}");

            PlayerMetadata playerMetadata = SaveLoadManager.LoadData();
            playerName.Value = playerMetadata.playerName;

            // Enable TowerSpawning UI
            towerSpawningUI.SetActive(true);
        }
    }

    public string GetUsername()
    {
        return playerName.Value.ToString();
    }

    [Rpc(SendTo.Server)]
    public void ChangeGameObjectNameRpc(string newGOName)
    {
        gameObject.name = newGOName;
    }

    public void OnPlayerNameChanged(FixedString512Bytes previousValue, FixedString512Bytes newValue)
    {
        m_PlayerName = newValue.ToString();
    }
}
