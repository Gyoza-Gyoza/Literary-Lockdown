using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;
    private List<NetworkObject> enemyList = new();
    public List<NetworkObject> EnemyList => enemyList;

    private float timer;
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer -= spawnInterval;
            SpawnEnemy();
        }
    }
    private void SpawnEnemy()
    {
        NetworkObject enemy = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(enemyPrefab);
        enemyList.Add(enemy);
        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;
    }
}
