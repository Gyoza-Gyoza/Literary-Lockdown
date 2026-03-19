using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TowerManager : NetworkBehaviour
{
    public List<Tower> towerList = new();
    public Stack<Tower> towerAttackingList = new();
    public List<NetworkObject> EnemyList
    {
        get
        {
            List<NetworkObject> currentEnemies = new();
            foreach (NetworkObject obj in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
            {
                if (obj.GetComponent<EnemyBehaviour>())
                {
                    currentEnemies.Add(obj);
                }
            }
            return currentEnemies;
        }
    }

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
        // TowerAttackHandler();
    }
    private void EnemyDetectionHandler()
    {
        foreach (Tower tower in towerList)
        {
            tower.target = null;

            float distanceToBeat = 0f;
            switch (tower.towerTargetStyle)
            {
                case Tower.targetStyle.Closest:
                    distanceToBeat = float.PositiveInfinity;
                    break;
                case Tower.targetStyle.Furthest:
                    distanceToBeat = float.NegativeInfinity;
                    break;
            }
            foreach (NetworkObject enemy in EnemyList)
            {
                float distance = Vector2.Distance(tower.transform.position, enemy.transform.position);
                if (distance <= tower.attackRange.Value) // Make sure it's within range
                {
                    switch (tower.towerTargetStyle)
                    {
                        case Tower.targetStyle.Closest:

                            if (distance < distanceToBeat) // Get closest enemy
                            {
                                distanceToBeat = distance;
                                tower.target = enemy;
                            }

                            break;
                        case Tower.targetStyle.Furthest:

                            if (distance > distanceToBeat) // Get furthest enemy
                            {
                                distanceToBeat = distance;
                                tower.target = enemy;
                            }

                            break;

                    }

                }
            }
        }
    }
    // private void TowerAttackHandler()
    // {
    //     foreach (Tower tower in towerList)
    //     {
    //         if (tower.target != null && tower.canAttack)
    //         {
    //             tower.Attack();
    //         }
    //     }
    // }
    public void AddTower(Tower tower) => towerList.Add(tower);
    public void RemoveTower(Tower tower) => towerList.Remove(tower);

    public void HideRangeOfTowers()
    {
        foreach(Tower t in towerList)
        {
            t.HideRange();
        }
    }
}