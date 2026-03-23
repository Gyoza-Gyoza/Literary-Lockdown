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
                // Do a check on location
                if (LocationManager.Instance.isLocationValid)
                {
                    localPlayer.playerReady.Value = true;
                    GetComponent<Image>().sprite = cancelImg;
                }
                else
                {
                    Debug.Log("Not at a valid location");
                }
                break;
        }

        LobbyManager.Instance.ForceReadyUpdate();
    }
}
