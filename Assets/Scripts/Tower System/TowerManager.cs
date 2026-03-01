using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TowerManager : NetworkBehaviour
{
    private List<Tower> towerList = new();
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
        TowerAttackHandler();
    }
    private void EnemyDetectionHandler()
    {
        foreach (Tower tower in towerList)
        {
            tower.target = null;

            float closestDistance = float.PositiveInfinity;

            foreach (NetworkObject enemy in EnemyList)
            {
                float distance = Vector2.Distance(tower.transform.position, enemy.transform.position);
                if (distance <= tower.attackRange.Value) // Make sure it's within range
                {
                    if (distance < closestDistance) // Get closest enemy
                    {
                        closestDistance = distance;
                        tower.target = enemy;
                    }
                }
            }
        }
    }

    // This function handles the attack logic for towers. 
    // It checks if the tower can attack and spawns a projectile facing the targeted enemy.
    // It also initializes the projectile's speed and damage based on the tower's stats.
    private void TowerAttackHandler()
    {
        foreach (Tower tower in towerList)
        {
            if (tower.target != null && tower.canAttack)
            {
                NetworkObject projectile = Instantiate(tower.projectilePrefab);
                projectile.transform.position = tower.transform.position;

                Vector2 direction = tower.target.transform.position - tower.transform.position;
                direction.Normalize();

                projectile.transform.rotation = Quaternion.FromToRotation(Vector2.up, direction);

                var bullet = projectile.GetComponentInChildren<Bullet>();
                if (bullet != null)
                {
                    bullet.speed.Value = tower.projectileSpeed.Value;
                    bullet.damage.Value = tower.damage.Value;
                }
                projectile.Spawn();
                tower.canAttack = false;
            }
        }
    }
    public void AddTower(Tower tower) => towerList.Add(tower);
    public void RemoveTower(Tower tower) => towerList.Remove(tower);
}