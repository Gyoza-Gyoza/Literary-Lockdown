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

public class NetworkThing : MonoBehaviour
{
    private NetworkManager m_NetworkManager;
    public GameObject NetworkScreen;
    [SerializeField] private GameObject[] spawnOnStart;

    public string m_LobbyJoinCode;
    private string m_PlayerName;
    private string targetIPAddr = "IP Addr";
    public TMP_InputField ipInput;
    public TMP_Text TMP_joinCodeText;

    private void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();

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


    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, Screen.height /2, Screen.width, Screen.height));

        //if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        //{
            //StartButtons();
       //}
        //else
        if (m_NetworkManager.IsServer || m_NetworkManager.IsClient || m_NetworkManager.IsHost)
        {
            if (NetworkScreen.activeSelf)
            {
                //ObjectivesManager.Instance.playersInLobby.Value++;
                NetworkScreen.SetActive(false);
            }
            if (m_NetworkManager.IsHost)
            {
                StatusLabels();
            }
        }

        GUILayout.EndArea();
    }

    

    public async void StartHost()
    {
        //m_NetworkManager.StartHost();
        //ObjectivesManager.Instance.playersInLobby.Value++;

        // Use only dtls or wss, udp is unencrypted and not recommended for production
        await StartHostWithRelay(4, "dtls");
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
        SetJoinCodeUI(m_LobbyJoinCode);

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
        SetJoinCodeUI(m_LobbyJoinCode);

        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }

    public void SetJoinCodeUI(string joinCode)
    {
        TMP_joinCodeText.text = $"Join Code: {joinCode}";
    }

    public async void ClientJoin()
    {
        //Connect(ipInput.text);

        // Use only dtls or wss, udp is unencrypted and not recommended for production
        try
        {
            await JoinLobbyWithRelay(ipInput.text, "dtls");
        }
        catch (ArgumentNullException e)
        {
            // Empty Join Code
            Debug.Log($"Failed to join relay lobby: Code cannot be empty");
            UIManager.Instance.ShowModalWindow("Join Failed", "Join code cannot be empty.");
        }
        catch (Exception e)
        {
            switch(e.Message)            
            {
                case string msg when msg.Contains("Bad Request"):
                    // Error code 400, likely due to invalid join code format
                    Debug.Log($"Failed to join relay lobby: Join code not found");
                    UIManager.Instance.ShowModalWindow("Join Failed", "Invalid Join Code.");
                    break;
                default:
                    Debug.LogWarning($"Some other error");
                    Debug.LogError($"{e.Message}");
                    UIManager.Instance.ShowModalWindow("Error", $"{e.Message}");
                    break;
            }
        }
    }

    public void LocalJoin()
    {
        Connect("localhost");
    }

    public void Connect(string enteredIP)
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(enteredIP, 7777);
        m_NetworkManager.StartClient();
        ObjectivesManager.Instance.playersInLobby.Value++;
    }

    private void StatusLabels()
    {
        var mode = m_NetworkManager.IsHost ?
            "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        //GUILayout.Label("IP Address: " + m_NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address);
        GUILayout.Label("Join Code: " + m_LobbyJoinCode);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log($"Client with ID {clientId} has disconnected.");

        // Check if the disconnected client is the local client
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // The local player was disconnected (e.g. Server shut down, lost connection)
            Debug.Log("You have been disconnected from the server.");
            SceneManager.LoadScene(0);
        }
        else
        {
            // Another player disconnected
            if (m_NetworkManager.IsServer)
            {
                // Logic for the server handling a dropped player (e.g. decrease player count)
            }
        }
    }
}
