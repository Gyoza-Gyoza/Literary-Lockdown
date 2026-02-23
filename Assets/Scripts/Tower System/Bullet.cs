using UnityEngine;
using Unity.Netcode;
public class Bullet : NetworkBehaviour
{
    public float speed = 0f;

    private void Update()
    {
        if (!IsServer) return;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
