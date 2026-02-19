using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GetClientUsername : MonoBehaviour
{
    public TextMeshProUGUI TMP_username;

    public string clientID;

    public void GetUsername()
    {
        // Get and set username for the player
        if (clientID != "")
        {
            TMP_username.text = GameObject.Find($"Player_{clientID}").GetComponent<PlayerClientController>().m_PlayerName;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
