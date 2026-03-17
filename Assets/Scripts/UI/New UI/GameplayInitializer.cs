using UnityEngine;
using Unity.Netcode;

public class GameplayInitializer : MonoBehaviour
{
    public GameObject[] initPrefabs;
    public static GameplayInitializer Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
