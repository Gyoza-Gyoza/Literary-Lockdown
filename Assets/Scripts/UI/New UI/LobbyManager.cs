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

    public static LobbyManager Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
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

    #region Player Connection

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) => AddPlayer(clientId);
        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) => RemovePlayer(clientId);

        playerList_GO = GameObject.Find("Player List");

        if (playerList_GO != null) 
            Debug.Log($"Found Player List GameObject: {playerList_GO.name}");
    }

    private async void OnPlayerListChange()
    { 
        for (int i = 0; i < playerList_GO.transform.childCount; i++)
        {
            Transform child = playerList_GO.transform.GetChild(i);

            TextMeshProUGUI playerName_TMPro = child.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

            // Check if we have a player for this UI slot index
            if (i < playerList.Count)
            {
                ulong clientId = playerList[i];
                GameObject playerObj = GameObject.Find($"Player_{clientId}");
                Debug.Log($"Found player object: {playerObj.name} for client ID: {clientId}");

                while (playerObj.GetComponent<PlayerClientController>().playerName.Value.ToString() == "Player Connected")
                {
                    await Task.Yield(); // Wait until the next frame
                }
                
                playerName_TMPro.text = playerObj.GetComponent<PlayerClientController>().playerName.Value.ToString();
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
        OnReadyChangeRpc();
    }

    private void RemovePlayer(ulong clientId)
    {
        playerList.Remove(clientId);
        OnPlayerListChange();
        OnReadyChangeRpc();
    }
    #endregion

    #region Ready Checker

    public void ForceReadyUpdate()
    {
        OnReadyChangeRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void OnReadyChangeRpc()
    {
        // Check if all players are ready
        for (int i = 0; i < playerList_GO.transform.childCount; i++)
        {
            Transform child = playerList_GO.transform.GetChild(i);

            TextMeshProUGUI playerReadyStat_TMPro = child.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();

            // Check if we have a player for this UI slot index
            if (i < playerList.Count)
            {
                ulong clientId = playerList[i];
                GameObject playerObj = GameObject.Find($"Player_{clientId}");
                Debug.Log($"Found player object: {playerObj.name} for client ID: {clientId}");

                switch(playerObj.GetComponent<PlayerClientController>().playerReady.Value)
                {
                    case true:
                        playerReadyStat_TMPro.text = "Ready";
                        playerReadyStat_TMPro.color = Color.green;
                        break;
                    case false:
                        playerReadyStat_TMPro.text = "Waiting";
                        playerReadyStat_TMPro.color = Color.red;
                        break;
                }
            }
            else
            {
                // Iteration is longer than the list, these are empty UI slots
                playerReadyStat_TMPro.text = ""; // Or clear it with ""
            }
        }

        // Server Logic to check if all players are ready to start the game
        foreach (NetworkClient playerClient in NetworkManager.ConnectedClientsList)
        {
            PlayerClientController cilent = playerClient.PlayerObject.GetComponent<PlayerClientController>();
            if (cilent.playerReady.Value == false)
            {
                return;
            }
        }

        NetworkManager.Singleton.SceneManager.LoadScene("Network Test", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    #endregion
}
