using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyButtons : MonoBehaviour
{
    public async void HostLobby()
    {
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
