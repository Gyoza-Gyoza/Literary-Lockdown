using UnityEngine;
using Unity.Netcode;

public class EnemyBehaviour : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 1.0f;
    public NetworkVariable<int> health = new NetworkVariable<int> (5);
    private NetworkVariable<int> currentWaypointIndex = new NetworkVariable<int>(0);
    private Vector2 targetPosition
    { get { return WaypointManager.Instance.waypoints[currentWaypointIndex.Value].position; } }

    private void Update()
    {
        Vector3 currentPos = transform.position;
        float move = movementSpeed * Time.deltaTime;
        if (Vector2.Distance(transform.position, targetPosition) >= 0.05f)
            transform.position = Vector3.MoveTowards(currentPos, targetPosition, move);
        else
        {
            if (IsHost)
                currentWaypointIndex.Value++;

            if (currentWaypointIndex.Value >= WaypointManager.Instance.waypoints.Length)
            {
                DestroyEnemyRpc();
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(int damage)
    {
        health.Value -= damage;

        //Take damage polish

        if (health.Value <= 0)
        {
            DestroyEnemyRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void DestroyEnemyRpc()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
        else
        {
            Destroy(gameObject);
        }
        ObjectivesManager.Instance.CaptureBooks();
    }
}
