using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
public class Player : NetworkBehaviour
{
    private NetworkVariable<Vector3> m_Position = new NetworkVariable<Vector3>();
    public NetworkVariable<Color> playerColor = new();
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    public void Move()
    {
        SubmitPositionRequestRpc();
    }

    [Rpc(SendTo.Server)]
    private void SubmitPositionRequestRpc(RpcParams rpcParams = default)
    {
        var randomPosition = GetRandomPositionOnPlane();
        transform.position = randomPosition;
        m_Position.Value = randomPosition;
    }
    [Rpc(SendTo.Server)]
    private void SetPlayerColorRpc(RpcParams rpcParams = default)
    {
        var randomColor = new Color(Random.value, Random.value, Random.value);
        playerColor.Value = randomColor;
        GetComponent<Renderer>().material.color = randomColor;
    }

    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }

    private void Update()
    {
        transform.position = m_Position.Value;
    }
}
