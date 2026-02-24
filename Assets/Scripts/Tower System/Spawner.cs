using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;

    private float timer;
    public static Spawner Instance { get; private set; }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            transform.position = WaypointManager.Instance.startPoint.position;
            transform.rotation = WaypointManager.Instance.startPoint.rotation;
        }
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }
    void Update()
    {
        if (ObjectivesManager.Instance.isGameStart()) 
        {
            if (!IsServer) return;
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer -= spawnInterval;
                SpawnEnemy();
            }
        }
    }
    private void SpawnEnemy()
    {
        NetworkObject enemy = Instantiate(enemyPrefab);
        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;
        enemy.Spawn();
    }
}
