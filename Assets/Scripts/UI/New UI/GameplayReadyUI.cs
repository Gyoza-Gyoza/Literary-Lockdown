using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class GameplayReadyUI : NetworkBehaviour
{
    private NetworkList<ulong> playerList = new NetworkList<ulong>();

    [SerializeField]
    private GameObject playerList_GO;

    public Sprite ready;
    public Sprite notReady;

    public static GameplayReadyUI Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        playerList_GO = GameObject.Find("Player List Gameplay");

        if (playerList_GO != null)
            Debug.Log($"Found Player List GameObject: {playerList_GO.name}");

        StartCoroutine(CheckStatus());
    }

    private async void OnPlayerListChange()
    {
        playerList.Clear();

        NetworkManager.Singleton.ConnectedClientsList.ToList().ForEach(client => {
            if (!playerList.Contains(client.ClientId))
            {
                playerList.Add(client.ClientId);
            }
        });


        for (int i = 0; i < playerList_GO.transform.childCount; i++)
        {
            Transform child = playerList_GO.transform.GetChild(i);

            TextMeshProUGUI playerName_TMPro = child.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

            // Check if we have a player for this UI slot index
            if (i < playerList.Count)
            {
                ulong clientId = playerList[i];
                GameObject playerObj = GameObject.Find($"Player_{clientId}");
                Debug.Log($"Found player object: {playerObj.name} for client ID: {clientId}");

                while (playerObj.GetComponent<PlayerClientController>().playerName.Value.ToString() == "Player Connected")
                {
                    Debug.Log($"Player object: {playerObj.name} name is not initialized yet, waiting...");
                    await Task.Yield(); // Wait until the next frame
                }

                Debug.Log($"{playerObj.name} Name: {playerObj.GetComponent<PlayerClientController>().playerName.Value.ToString()}, Setting to {child.GetChild(0).name} in {child.name}");
                playerName_TMPro.text = playerObj.GetComponent<PlayerClientController>().playerName.Value.ToString();
            }
            else
            {
                // Iteration is longer than the list, these are empty UI slots
                playerName_TMPro.text = ""; // Or clear it with ""
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void OnReadyChangeRpc()
    {
        playerList.Clear();

        NetworkManager.Singleton.ConnectedClientsList.ToList().ForEach(client => {
            if (!playerList.Contains(client.ClientId))
            {
                playerList.Add(client.ClientId);
            }
        });

        // Check if all players are ready
        for (int i = 0; i < playerList_GO.transform.childCount; i++)
        {
            Transform child = playerList_GO.transform.GetChild(i);

            Image playerReadyStat = child.GetChild(1).GetComponentInChildren<Image>();

            // Check if we have a player for this UI slot index
            if (i < playerList.Count)
            {
                ulong clientId = playerList[i];
                GameObject playerObj = GameObject.Find($"Player_{clientId}");
                Debug.Log($"Found player object: {playerObj.name} for client ID: {clientId}");

                // Change the image sprite accordingly
                switch (playerObj.GetComponent<PlayerClientController>().playerReady.Value)
                {
                    case true:
                        playerReadyStat.sprite = ready;
                        break;
                    case false:
                        playerReadyStat.sprite = notReady;
                        break;
                }
            }
            else
            {
                // Iteration is longer than the list, these are empty UI slots
                // TODO: Hide objects???
                playerReadyStat.sprite = null;
            }
        }
    }

    public IEnumerator CheckStatus()
    {
        while (true)
        {
            // Wait for 1 second before checking to let initialization finish
            // and prevent flooding if an error occurs
            yield return new WaitForSeconds(0.5f);

            if (playerList_GO != null)
            {
                OnPlayerListChange();
                OnReadyChangeRpc();
            }
        }
    }
}
