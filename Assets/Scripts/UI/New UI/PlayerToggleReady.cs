using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerToggleReady : MonoBehaviour
{
    public Sprite readyImg;
    public Sprite cancelImg;

    public void ToggleReady()
    {
        PlayerClientController localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>();

        switch (localPlayer.playerReady.Value)
        {
            case true:
                localPlayer.playerReady.Value = false;
                GetComponent<Image>().sprite = readyImg;
                break;
            case false:
                localPlayer.playerReady.Value = true;
                GetComponent<Image>().sprite = cancelImg;
                break;
        }

        LobbyManager.Instance.ForceReadyUpdate();
    }
}
