using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    public GameObject playerListPrefab;

    [SerializeField]
    private NetworkManager m_NetworkManager;
    public Dictionary<string, GameObject> m_PlayerList = new Dictionary<string, GameObject>();

    public void Awake()
    {
        m_NetworkManager.OnConnectionEvent += PlayerConnection;
    }

    private void PlayerConnection(NetworkManager manager, ConnectionEventData data)
    {
        switch (data.EventType)
        {
            case (ConnectionEvent.ClientConnected):
                Debug.Log($"Player Connected | ClientId={data.ClientId} ConnectionEvent={data.EventType}");

                // Add the new player to the list
                GameObject newPlayer = Instantiate(playerListPrefab, transform);
                newPlayer.GetComponent<GetClientUsername>().clientID = data.ClientId.ToString();
                m_PlayerList.Add(data.ClientId.ToString(), newPlayer);
                break;

            case (ConnectionEvent.ClientDisconnected):
                Debug.Log($"Player Disconnected | ClientId={data.ClientId} ConnectionEvent={data.EventType}");
                
                // Remove the player from the list
                if (m_PlayerList.TryGetValue(data.ClientId.ToString(), out GameObject player))
                {
                    Destroy(player);
                    m_PlayerList.Remove(data.ClientId.ToString());
                }
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
