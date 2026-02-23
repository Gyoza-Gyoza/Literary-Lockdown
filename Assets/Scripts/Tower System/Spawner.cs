using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;
    private List<EnemyBehaviour> enemyList = new();
    public List<EnemyBehaviour> EnemyList => enemyList;

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
        enemyList.Add(enemy.GetComponent<EnemyBehaviour>());
        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;
    }
}
