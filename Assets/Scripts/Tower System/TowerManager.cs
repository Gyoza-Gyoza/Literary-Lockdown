using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using Unity.Netcode;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private GameObject towerTemplate;
    [SerializeField] private int maxTowers; 
    private List<Tower> towerList = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) CreateTower("Hansel");
    }
    public GameObject CreateTower(string name)
    {
        GameObject result = Instantiate(towerTemplate);
        Tower tower = result.AddComponent(Type.GetType(name)) as Tower;

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
        //tower.InitializeObject(towerData.Sprite, towerData.Stats);
        towerList.Add(tower);

        result.GetComponent<NetworkObject>().Spawn();
        return result;
    }
}
