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

    [Rpc(SendTo.Everyone)]
    public void ChangeGameObjectNameRpc(string newGOName)
    {
        gameObject.name = newGOName;
    }

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += OnPlayerNameChanged;

        if (IsOwner)
        {
            ChangeGameObjectNameRpc($"Player_{NetworkManager.Singleton.LocalClientId.ToString()}");

            PlayerMetadata playerMetadata = SaveLoadManager.LoadData();
            playerName.Value = playerMetadata.playerName;
        }
    }

    public void Awake()
    {
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerNameChanged(FixedString512Bytes previousValue, FixedString512Bytes newValue)
    {
        m_PlayerName = newValue.ToString();
    }
}
