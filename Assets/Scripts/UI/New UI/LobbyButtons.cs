using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyButtons : MonoBehaviour
{
    public GameObject joinLobbyModal;

    private string m_lobbyCode;
    public TMP_InputField lobbyCodeInputField;

    public void Update()
    {
        m_lobbyCode = lobbyCodeInputField.text;
    }

    public async void HostLobby()
    {
        await SceneManager.LoadSceneAsync("Game Lobby");

        // Start Host Function
        GameObject.FindFirstObjectByType<NetworkHandler>().StartHost();
    }

    public void ShowJoinLobbyModal()
    {
        joinLobbyModal.SetActive(true);
    }

    public void HideJoinLobbyModal()
    {
        joinLobbyModal.SetActive(false);
    }

    public async void JoinLobby()
    {
        await SceneManager.LoadSceneAsync("Game Lobby");
        await GameObject.FindFirstObjectByType<NetworkHandler>().JoinLobbyWithRelay(m_lobbyCode, "dtls");
    }
}
