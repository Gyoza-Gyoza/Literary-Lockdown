using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyButtons : MonoBehaviour
{
    public Image hostButton;
    public Image joinButton;

    public Sprite hostUnavailSprite;
    public Sprite hostAvailSprite;
    public Sprite joinUnavailSprite;
    public Sprite joinAvailSprite;

    private bool locked = false;

    private void Update()
    {
        if (LocationManager.Instance == null || !LocationManager.Instance.isLocationValid)
        {
            SetButtonsActive(false);
        }
        else
        {
            SetButtonsActive(true);
        }
    }

    private void SetButtonsActive(bool active)
    {
        hostButton.sprite = active ? hostAvailSprite : hostUnavailSprite;
        joinButton.sprite = active ? joinAvailSprite : joinUnavailSprite;
        locked = !active;
    }

    public async void HostLobby()
    {
        if (locked) return; 
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
        if (locked) return;

        await SceneManager.LoadSceneAsync("Game Lobby");
        
        GameObject.Find("Loading Screen").SetActive(false);
        GameObject.FindFirstObjectByType<NetworkHandler>().joinLobbyModal.SetActive(true);
    }
}
