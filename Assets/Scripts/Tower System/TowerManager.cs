using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TowerManager : NetworkBehaviour
{
    private List<Tower> towerList = new();
    private List<EnemyBehaviour> enemyList = new();

    public static TowerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Update()
    {
        if (!IsServer) return;
        EnemyDetectionHandler();
        TowerAttackHandler();
    }
    private void EnemyDetectionHandler()
    {
        foreach (Tower tower in towerList)
        {
            if (tower.target != null) continue;
            else
            {
                foreach (EnemyBehaviour enemy in enemyList)
                {
                    if (Vector2.Distance(tower.transform.position, enemy.transform.position) <= tower.attackRange)
                    {
                        tower.target = enemy;
                        break;
                    }
                }
            }
        }
    }
    private void TowerAttackHandler()
    {
        foreach (Tower tower in towerList)
        {
            if (tower.target != null && tower.canAttack)
            {
                NetworkObject projectile = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(tower.projectilePrefab);
                projectile.transform.position = tower.transform.position;
                projectile.transform.LookAt(tower.target.transform);
                Debug.Log($"Attacking");
            }
        }
    }
    public void AddTower(Tower tower) => towerList.Add(tower);
    public void RemoveTower(Tower tower) => towerList.Remove(tower);

    //[SerializeField] private GameObject towerTemplate;
    //[SerializeField] private int maxTowers; 

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P)) CreateTowerRPC("Hansel");
    //}
    //[Rpc(SendTo.Server)]
    //public void CreateTowerRPC(string name)
    //{
    //    if (IsServer)
    //    {
    //        GameObject result = Instantiate(towerTemplate);
    //        result.AddComponent(GetTowerType(name));

    //        TowerData towerData = new();
    //        if (Database.instance.database.TryGetValue("Towers", out List<object> towerObjects))
    //        {
    //            foreach (object obj in towerObjects)
    //            {
    //                TowerData data = (TowerData)obj;
    //                if (data.Name == name)
    //                {
    //                    towerData = data;
    //                    break;
    //                }
    //            }
    //        }

    //        //Add initialization logic here
    //        //tower.InitializeObject(towerData.Sprite, towerData.Stats);
    //        towerList.Add(result.GetComponent<Tower>());

    //        result.GetComponent<NetworkObject>().Spawn();
    //    }
    //}
    //private Type GetTowerType(string name)
    //{
    //    switch (name)
    //    {
    //        case "Hansel":
    //            return typeof(Hansel);
    //        case "Gretel":
    //            return typeof(Gretel);
    //        default:
    //            throw new ArgumentException($"Tower type '{name}' not recognized.");
    //    }
    //}
}
public enum TowerType
{
    Hansel,
    Gretel
}