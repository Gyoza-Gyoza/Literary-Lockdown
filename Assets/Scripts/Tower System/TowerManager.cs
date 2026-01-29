using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TowerManager : NetworkBehaviour
{
    [SerializeField] private GameObject towerTemplate;
    [SerializeField] private int maxTowers; 
    private List<Tower> towerList = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) CreateTowerRPC("Hansel");
    }
    [Rpc(SendTo.Server)]
    public void CreateTowerRPC(string name)
    {
        if (IsServer)
        {
            GameObject result = Instantiate(towerTemplate);
            result.AddComponent(GetTowerType(name));

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
            towerList.Add(result.GetComponent<Tower>());

            result.GetComponent<NetworkObject>().Spawn();
        }
    }
    private Type GetTowerType(string name)
    {
        switch (name)
        {
            case "Hansel":
                return typeof(Hansel);
            case "Gretel":
                return typeof(Gretel);
            default:
                throw new ArgumentException($"Tower type '{name}' not recognized.");
        }
    }
}
public enum TowerType
{
    Hansel,
    Gretel
}