using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerClientController : NetworkBehaviour
{
    [Header("Player Stats")]
    public NetworkVariable<FixedString512Bytes> playerName = new NetworkVariable<FixedString512Bytes>("Player Connected", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField]
    private NetworkVariable<FixedString512Bytes> m_GameObjectName = new NetworkVariable<FixedString512Bytes>("Default Name", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private ulong playerID;

    [SerializeField]
    public NetworkVariable<bool> playerReady = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Tower Stats")]
    public int maxTowers;
    public NetworkVariable<int> currentTowers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public List<NetworkObject> towerPrefabList;

    [Header("UI")]
    public GameObject towerSpawningUI;

    public void Awake()
    {
        // Get the UI GameObject reference
        //towerSpawningUI = UIManager.Instance.TowerSpawner;
    }

    public override void OnNetworkSpawn()
    {
        currentTowers.OnValueChanged += OnCurrentTowersChangedRpc;
        m_GameObjectName.OnValueChanged += ChangeGameObjectNameRpc;
        playerReady.OnValueChanged += UpdateReadyStatusRpc;

        if (IsOwner)
        {
            m_GameObjectName.Value = $"Player_{NetworkManager.Singleton.LocalClientId.ToString()}";

            // Get Player name
            PlayerMetadata playerMetadata = SaveLoadManager.LoadData();
            playerName.Value = playerMetadata.playerName;

            playerID = NetworkManager.Singleton.LocalClientId;

            // Enable TowerSpawning UI
            //towerSpawningUI.SetActive(true);
            //UIManager.Instance.ShowPlayerReadyUI();
        }

        ChangeGameObjectNameRpc("" , m_GameObjectName.Value);
    }

    #region Tower Handler

    public bool TrySpawnTower(int towerIndex)
    {
        if (currentTowers.Value >= maxTowers)
        {
            Debug.Log("Max towers reached. Cannot spawn more.");
            UIManager.Instance.ShowModalWindow("Tower Limit Reached", $"You can only have {maxTowers} towers at a time.");
            return false;
        }

        currentTowers.Value += 1;

        // Spawn the tower on the server
        SpawnTowerRpc(towerIndex);

        return true;
    }

    [Rpc(SendTo.Server)]
    public void SpawnTowerServerRpc(int towerIndex, ulong clientID)
    {
        //GameObject towerToSpawn = Instantiate(towerPrefabList[towerIndex]);
        NetworkObject towerToSpawn = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(towerPrefabList[towerIndex], clientID);

        // Rename the tower
        Tower tower = towerToSpawn.GetComponent<Tower>(); 
        tower.m_TowerName.Value = GameObject.Find($"Player_{clientID}").GetComponent<PlayerClientController>().playerName.Value;
        TowerManager.Instance.AddTower(tower);
        Debug.Log($"Tower spawned for player {clientID}. Current towers: {currentTowers.Value}");
    }


    [Rpc(SendTo.Owner)]
    public void SpawnTowerRpc(int towerIndex)
    {
        SpawnTowerServerRpc(towerIndex, playerID);
    }

    public void DestroyTowerRpc(GameObject gameObject)
    {
        if (gameObject.GetComponent<NetworkObject>().IsOwner)
        {
            gameObject.GetComponent<Tower>().DestroyTowerRpc();
            TowerManager.Instance.RemoveTower(gameObject.GetComponent<Tower>());
            currentTowers.Value -= 1;
        }
    }
    #endregion

    public string GetUsername()
    {
        return playerName.Value.ToString();
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeGameObjectNameRpc(FixedString512Bytes previousValue, FixedString512Bytes newValue)
    {
        gameObject.name = newValue.ToString();
    }

    public void UpdateReadyStatusRpc(bool oldValue, bool newValue)
    {
        playerReady.Value = newValue;
        LobbyManager.Instance.ForceReadyUpdate();
    }

    [Rpc(SendTo.Server)]
    public void OnCurrentTowersChangedRpc(int previousValue, int newValue)
    {
        currentTowers.Value = newValue;
    }
}
