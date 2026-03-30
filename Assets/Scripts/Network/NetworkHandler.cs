using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkHandler : MonoBehaviour
{
    private NetworkManager m_NetworkManager;
    [SerializeField] private GameObject[] spawnOnStart;

    public GameObject joinLobbyModal;
    public TMP_InputField lobbyCode_Input;
    public string m_LobbyJoinCode;

    [SerializeField]
    private TextMeshProUGUI TMPro_LobbyCode;
    private string m_PlayerName;

    public static NetworkHandler Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this; 
        else Destroy(gameObject);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            foreach (GameObject obj in spawnOnStart)
            {
                NetworkObject networkObject = Instantiate(obj).GetComponent<NetworkObject>();
                networkObject.Spawn();
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    #region Lobby Creation
    public async void StartHost()
    {
        //m_NetworkManager.StartHost();
        //ObjectivesManager.Instance.playersInLobby.Value++;

        // Use only dtls or wss, udp is unencrypted and not recommended for production
        await StartHostWithRelay(4, "dtls");
    }

    public async void StartClient()
    {
        try
        {
            m_LobbyJoinCode = lobbyCode_Input.text;
            Debug.Log($"Attempting to join lobby with code: {m_LobbyJoinCode}");
            bool connectionAttempt = await JoinLobbyWithRelay(m_LobbyJoinCode, "dtls");

            if (connectionAttempt)
            {
                joinLobbyModal.SetActive(false);
            }
        }
        catch (RelayServiceException e)
        {
            // Specifically catches Relay errors (e.g., Invalid code, lobby not found)
            Debug.LogError($"Relay Service Error: {e.Message}");
        }
        catch (Exception e)
        {
            // Catches any other unexpected errors
            Debug.LogError($"An unexpected error occurred: {e.Message}");
        }
    }

    public async Task<string> StartHostWithRelay(int maxConnections, string connectionType)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));

        // Get the lobby join code and display it to the user
        m_LobbyJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        SetLobbyCode(m_LobbyJoinCode);

        return NetworkManager.Singleton.StartHost() ? m_LobbyJoinCode : null;
    }

    public async Task<bool> JoinLobbyWithRelay(string joinCode, string connectionType)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));

        m_LobbyJoinCode = joinCode;
        SetLobbyCode(joinCode);

        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }


    #endregion

    public void SetLobbyCode(string code)
    {
        TMPro_LobbyCode.text = $"CODE: {code}";
    }


    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log($"Client with ID {clientId} has disconnected.");

        // Check if the disconnected client is the local client
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Destroy(NetworkManager.Singleton.gameObject);

            // The local player was disconnected (e.g. Server shut down, lost connection)
            Debug.Log("You have been disconnected from the server.");
            SceneManager.LoadScene("Raid Menu");
        }
        else
        {
            // Another player disconnected
            if (NetworkManager.Singleton.IsServer)
            {
                // Logic for the server handling a dropped player (e.g. decrease player count)
            }
        }
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
}
