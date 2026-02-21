using System;
using System.Collections.Generic;
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

    public List<GameObject> towerPrefabList;

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
        currentTowers.OnValueChanged += OnCurrentTowersChangedRpc;

        if (IsOwner)
        {
            ChangeGameObjectNameRpc($"Player_{NetworkManager.Singleton.LocalClientId.ToString()}");

            PlayerMetadata playerMetadata = SaveLoadManager.LoadData();
            playerName.Value = playerMetadata.playerName;

            // Enable TowerSpawning UI
            towerSpawningUI.SetActive(true);
        }
    }

    #region Tower Handler

    public void TrySpawnTower(int towerIndex)
    {
        if (currentTowers.Value >= maxTowers)
        {
            Debug.Log("Max towers reached. Cannot spawn more.");
            return;
        }

        currentTowers.Value += 1;

        // Spawn the tower on the server
        SpawnTowerRpc(towerIndex);
    }

    [Rpc(SendTo.Server)]
    public void SpawnTowerRpc(int towerIndex)
    {
        GameObject towerToSpawn = Instantiate(towerPrefabList[towerIndex]);

        // Edit tower Stats
        towerToSpawn.name = $"{towerToSpawn.name.Replace("(Clone)", "")}_{m_PlayerName}";
        towerToSpawn.GetComponent<NetworkObject>().Spawn();
    }    
    #endregion


    public string GetUsername()
    {
        return playerName.Value.ToString();
    }

    [Rpc(SendTo.Server)]
    public void ChangeGameObjectNameRpc(string newGOName)
    {
        gameObject.name = newGOName;
    }

    [Rpc(SendTo.Server)]
    public void OnCurrentTowersChangedRpc(int previousValue, int newValue)
    {
        currentTowers.Value = newValue;
    }

    public void OnPlayerNameChanged(FixedString512Bytes previousValue, FixedString512Bytes newValue)
    {
        m_PlayerName = newValue.ToString();
    }
}
