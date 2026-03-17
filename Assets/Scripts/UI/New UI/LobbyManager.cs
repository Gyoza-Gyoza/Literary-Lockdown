using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


public class LobbyManager : NetworkBehaviour
{

    private NetworkList<ulong> playerList = new NetworkList<ulong>();
    private GameObject playerList_GO;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) => AddPlayer(clientId);
        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) => RemovePlayer(clientId);

        playerList_GO = GameObject.Find("Player List");

        if (playerList_GO != null) 
            Debug.Log($"Found Player List GameObject: {playerList_GO.name}");
    }

    private void OnPlayerListChange()
    { 
        for (int i = 0; i < playerList_GO.transform.childCount; i++)
        {
            Transform child = playerList_GO.transform.GetChild(i);

            TextMeshProUGUI playerName_TMPro = child.GetComponentInChildren<TextMeshProUGUI>();

            // Check if we have a player for this UI slot index
            if (i < playerList.Count)
            {
                ulong clientId = playerList[i];
                GameObject playerObj = GameObject.Find($"Player_{clientId}");
                Debug.Log($"Found player object: {playerObj.name} for client ID: {clientId}");

                if (playerObj != null)
                {
                    playerName_TMPro.text = playerObj.GetComponent<PlayerClientController>().playerName.Value.ToString();
                }
            }
            else
            {
                // Iteration is longer than the list, these are empty UI slots
                playerName_TMPro.text = "Waiting..."; // Or clear it with ""
            }
        }
    }

    private async void AddPlayer(ulong clientId)
    {
        Debug.Log($"Adding player with client ID: {clientId} to the lobby.");
        playerList.Add(clientId);

        while (GameObject.Find($"Player_{clientId}") == null)
        {
            await Task.Yield(); // Wait until the next frame
        }

        OnPlayerListChange();
    }

    private void RemovePlayer(ulong clientId)
    {
        playerList.Remove(clientId);
        OnPlayerListChange();
    }


}
