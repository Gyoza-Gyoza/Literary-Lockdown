using Unity.Netcode;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    private Player m_localPlayer;
    private NetworkManager m_networkManager;


    public void Awake()
    {
        
    }

    public void SelectCharacter(int id)
    {
        m_networkManager = FindFirstObjectByType<NetworkManager>();

        var playerObject = m_networkManager.SpawnManager.GetLocalPlayerObject();
        m_localPlayer = playerObject.GetComponent<Player>();
        m_localPlayer.SetSpriteRpc(id);

        // Close menu
        gameObject.SetActive(false);
    }
}
