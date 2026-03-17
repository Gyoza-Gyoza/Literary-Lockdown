using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyButtons : MonoBehaviour
{
    public async void HostLobby()
    {
        // Set raid details in lobby details script
        RaidSettings raidSettings = GameObject.FindFirstObjectByType<RaidSettings>();
        raidSettings.LockInSettings();

        LobbyDetails lobbyDetails = GameObject.FindFirstObjectByType<LobbyDetails>();
        DontDestroyOnLoad(lobbyDetails.gameObject);

        await SceneManager.LoadSceneAsync("Game Lobby");

        // Start Host Function
        GameObject.FindFirstObjectByType<NetworkHandler>().StartHost();
    }

    public async void JoinLobby()
    {
        await SceneManager.LoadSceneAsync("Game Lobby");
        
        GameObject.FindFirstObjectByType<NetworkHandler>().joinLobbyModal.SetActive(true);
    }
}
