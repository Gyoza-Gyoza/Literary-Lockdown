using UnityEngine;
using Unity.Netcode;
public class Bullet : NetworkBehaviour
{
    [SerializeField] private float lifetime = 1f;
    [HideInInspector] public float speed = 0f;
    [HideInInspector] public int damage = 0;
    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.TryGetComponent<EnemyBehaviour>(out EnemyBehaviour enemy))
        {
            enemy.TakeDamage(damage);
            DestroyBullet();
        }
    }
}
