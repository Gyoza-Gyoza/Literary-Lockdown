using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.RayTracingAccelerationStructure;


public class LobbyManager : NetworkBehaviour
{

    private NetworkList<ulong> playerList = new NetworkList<ulong>();
    private GameObject playerList_GO;
    private GameObject loadingScreen;
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

        loadingScreen = GameObject.Find("Loading Screen");

        // Hardcoding this...
        GameObject.Find("Btn Leave").GetComponent<Button>().onClick.AddListener(LeaveSession);
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

        if (loadingScreen.activeSelf)
            loadingScreen.SetActive(false);
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

    public void LeaveSession()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        if (FindFirstObjectByType<LobbyDetails>() != null)
        {
            Destroy(FindFirstObjectByType<LobbyDetails>().gameObject);
        }

        Destroy(NetworkManager.Singleton.gameObject);

        SceneManager.LoadScene("Raid Menu");
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

        if (IsHost)
        {
            // Server Logic to check if all players are ready to start the game
            foreach (NetworkClient playerClient in NetworkManager.ConnectedClientsList)
            {
                PlayerClientController cilent = playerClient.PlayerObject.GetComponent<PlayerClientController>();
                if (cilent.playerReady.Value == false)
                {
                    return;
                }
            }

            // What is the time complexity of this whole funcction? Its 2 for loops nested within another for loop...
            foreach (NetworkClient playerClient in NetworkManager.ConnectedClientsList)
            {
                // Reset the ready status for the next scene
                PlayerClientController cilent = playerClient.PlayerObject.GetComponent<PlayerClientController>();
                cilent.SetReadyStatusRpc(false);
            }

            SetLoadingScreenRpc();

            NetworkManager.Singleton.SceneManager.LoadScene("Network Test", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SetLoadingScreenRpc()
    {
        loadingScreen.SetActive(true);
    }

    #endregion
}
