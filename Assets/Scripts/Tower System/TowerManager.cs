using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private GameObject towerTemplate;
    [SerializeField] private int maxTowers; 
    private List<Tower> towerList = new();

    private void Start()
    {

    }
    public GameObject CreateTower(string name)
    {
        GameObject result = Instantiate(towerTemplate);
        Tower tower = result.GetComponent<Tower>();
        TowerData towerData = new(); 
        if (Database.instance.database.TryGetValue("Towers", out List<object> towerObjects))
        {
            foreach (object obj in towerObjects)
            {
                TowerData data = (TowerData)obj;
                if (data.Name == name)
                {
                    towerData = data;
                    break;
                }
            }
        }

        //Add initialization logic here
        

        return result;
    }
}
