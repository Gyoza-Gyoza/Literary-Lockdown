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
        m_NetworkManager.OnConnectionEvent += OnConnectionEvent;
    }

    private void OnConnectionEvent(NetworkManager network, ConnectionEventData data)
    {
        //Brute Force method
        m_PlayerList.Clear();

        foreach (NetworkClient client in m_NetworkManager.ConnectedClientsList)
        {
            if (!m_PlayerList.ContainsKey(client.ClientId.ToString()))
            {
                GameObject playerListItem = Instantiate(playerListPrefab, transform);
                playerListItem.GetComponent<GetClientUsername>().clientID = $"{client.ClientId}";
                m_PlayerList.Add(client.ClientId.ToString(), playerListItem);

                playerListItem.GetComponent<GetClientUsername>().GetUsername();
            }
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
