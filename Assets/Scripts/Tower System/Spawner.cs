using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;

    public float frequencyChangePerMin = 0.04f;

    private float counter;
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
            counter += Time.deltaTime;
            timer += Time.deltaTime;

            if (timer >= 60f)
            {
                timer -= 60f;
                spawnInterval -= frequencyChangePerMin;
            }

            if (counter >= spawnInterval)
            {
                counter -= spawnInterval;
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
