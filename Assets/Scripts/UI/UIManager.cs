using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject TowerSpawner;
    public GameObject TowerControlPanel;

    public GameObject seletedTower;


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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TowerControlPanel.activeSelf && seletedTower != null)
        {
            Vector3 targetPosition = Camera.main.WorldToScreenPoint(seletedTower.transform.position);
            TowerControlPanel.transform.position = targetPosition;
        }
    }
}
