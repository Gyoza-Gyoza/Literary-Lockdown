using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;


    public float extraFreqPerDifficulty = 0.5f;
    public float frequencyChangePerMin = 0.02f;

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

            //Debug.Log("interval is at " + spawnInterval + ", extra freq per difficulty is at " + extraFreqPerDifficulty + ", difficulty value is " + ObjectivesManager.Instance.difficulty.Value);
            //Debug.Log("final calc " + (spawnInterval - (extraFreqPerDifficulty * (float)ObjectivesManager.Instance.difficulty.Value)));

            if (timer >= 60f)
            {
                timer -= 60f;
                spawnInterval -= frequencyChangePerMin;
            }

            float finalcalc = (spawnInterval - (extraFreqPerDifficulty * (float)ObjectivesManager.Instance.difficulty.Value));

            if (counter >= finalcalc)
            {
                counter -= finalcalc;
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
