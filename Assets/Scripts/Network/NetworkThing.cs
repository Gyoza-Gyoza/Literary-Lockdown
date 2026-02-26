using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkThing : MonoBehaviour
{
    private NetworkManager m_NetworkManager;
    [SerializeField] private GameObject[] spawnOnStart;

    private string m_PlayerName;
    private string targetIPAddr = "";

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

        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
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
    }

    private void StatusLabels()
    {
        var mode = m_NetworkManager.IsHost ?
            "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("IP Address: " + m_NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address);
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
