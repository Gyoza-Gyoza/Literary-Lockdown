using Unity.Netcode;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    private Tower m_localPlayer;
    private NetworkManager m_networkManager;


    public void Awake()
    {
        
    }

    public void TrySpawnTower(int towerIndex)
    {
        if ( NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>().TrySpawnTower(towerIndex))
        {
            //close the UI
        }
        else { 
         
            //display an error ig
               
        }
    }
}
