using UnityEngine;
using Unity.Netcode;

public class EnemyBehaviour : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private int health = 2;
    private void Update()
    {
        Vector3 currentPos = transform.position;
        float move = movementSpeed * Time.deltaTime;
        transform.position = new Vector3(currentPos.x, currentPos.y + move, currentPos.z);
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            DestroyEnemy();
        }
    }
    public void DestroyEnemy()
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
    }
}
