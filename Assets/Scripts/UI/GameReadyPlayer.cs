using System.Xml.Schema;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameReadyPlayer : MonoBehaviour
{

    public Sprite sprite_Ready;
    public Sprite sprite_Cancel;

    public Image btnImage;

    public TextMeshPro currentReadyText;
    public TextMeshPro playersInSessionText;

    //private int playersReady = 0;

    public void TogglePlayerReady()
    {
        PlayerClientController playerClient = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>();
        playerClient.playerReady.Value = !playerClient.playerReady.Value;

        if (playerClient.playerReady.Value == true)
        {
            btnImage.sprite = sprite_Cancel;
            //playersReady++;
        }
        else
        {
            btnImage.sprite = sprite_Ready;
            //playersReady--;
        }
    }

    public void UpdatePlayersReady()
    {

    }

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void GameStarted()
    {
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (ObjectivesManager.Instance.isGameStart())
        {
            GameStarted();
        }
    }
}
