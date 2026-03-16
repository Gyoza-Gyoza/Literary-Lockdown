using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using NUnit.Framework;
public class Bullet : NetworkBehaviour
{
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private bool destroyOnHit;
    [HideInInspector] public NetworkVariable<float> speed;
    [HideInInspector] public NetworkVariable<int> damage;
    private List<EnemyBehaviour> hitEnemies = new();
    private void Update()
    {
        transform.Translate(Vector3.up * speed.Value * Time.deltaTime);

        if (lifetime <= 0f)
        {
            DestroyBullet();
        }
        else
        {
            lifetime -= Time.deltaTime;
        }
    }
    public void DestroyBullet()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn();
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsServer) return;
        if (collision.gameObject.TryGetComponent<EnemyBehaviour>(out EnemyBehaviour enemy))
        {
            if (!hitEnemies.Contains(enemy))
            {
                enemy.TakeDamageRpc(damage.Value);
                Debug.Log("Hit enemy");
                hitEnemies.Add(enemy);
                if (destroyOnHit) DestroyBullet();
            }
        }
    }
}
