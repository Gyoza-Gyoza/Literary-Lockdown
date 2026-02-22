using UnityEngine;
using Unity.Netcode;

public class EnemyBehaviour : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 1.0f;
    private void Update()
    {
        Vector3 currentPos = transform.position;
        float move = movementSpeed * Time.deltaTime;
        transform.position = new Vector3(currentPos.x, currentPos.y + move, currentPos.z);
    }
}
