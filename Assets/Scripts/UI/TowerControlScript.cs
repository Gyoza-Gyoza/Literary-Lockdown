using Unity.Netcode;
using UnityEngine;

public class TowerControlScript : MonoBehaviour
{
    public UIManager m_UIManager;

    public void Awake()
    {
        m_UIManager = UIManager.Instance;
    }

    public void TryMoveTower()
    {
        if (m_UIManager.seletedTower != null)
        {
            m_UIManager.TowerControlPanel.SetActive(false);
            m_UIManager.seletedTower.GetComponent<Tower>().StartMovementRpc();
        }
    }

    public void TryDeleteTower()
    {
        if (m_UIManager.seletedTower != null)
        {
            m_UIManager.TowerControlPanel.SetActive(false);
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>().DestoryTowerRpc(m_UIManager.seletedTower);
        }
    }
}
