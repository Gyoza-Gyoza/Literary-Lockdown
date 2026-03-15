using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Collections;

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
        await JoinLobbyWithRelay(ipInput.text, "dtls");
    }

    public void LocalJoin()
    {
        Connect("localhost");
    }

    private void StartButtons()
    {
        if (GUILayout.Button("Host")) m_NetworkManager.StartHost();
        if (GUILayout.Button("Server")) m_NetworkManager.StartServer();
        targetIPAddr = GUILayout.TextField(targetIPAddr, 25);
        if (GUILayout.Button("Client Join")) Connect(targetIPAddr);
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

    private void SubmitNewPosition()
    {
        if (GUILayout.Button(m_NetworkManager.IsServer ? "Move" : "Request Position Change"))
        {
            if (m_NetworkManager.IsServer && !m_NetworkManager.IsClient)
            {
                foreach (ulong uid in m_NetworkManager.ConnectedClientsIds)
                {
                    m_NetworkManager.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Tower>();
                }
            }
            else
            {
                var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
                var player = playerObject.GetComponent<Tower>();
            }
        }
    }
}
