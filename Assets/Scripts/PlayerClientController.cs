using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerClientController : NetworkBehaviour
{
    [Header("Player Stats")]
    private NetworkVariable<FixedString512Bytes> playerName = new NetworkVariable<FixedString512Bytes>("Player Connected", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField]
    private ulong playerID;
    public string m_PlayerName;

    [Header("Tower Stats")]
    public int maxTowers;
    public NetworkVariable<int> currentTowers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public List<NetworkObject> towerPrefabList;

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

            playerID = NetworkManager.Singleton.LocalClientId;

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
        //GameObject towerToSpawn = Instantiate(towerPrefabList[towerIndex]);
        NetworkObject towerToSpawn = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(towerPrefabList[towerIndex], playerID);

        // Edit tower Stats
        towerToSpawn.name = $"{towerToSpawn.name.Replace("(Clone)", "")}_{m_PlayerName}";
        towerToSpawn.GetComponentInChildren<TextMeshPro>().text = m_PlayerName;
    }


    public void DestroyTowerRpc(GameObject gameObject)
    {
        if (gameObject.GetComponent<NetworkObject>().IsOwner)
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
            currentTowers.Value -= 1;
        }
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
