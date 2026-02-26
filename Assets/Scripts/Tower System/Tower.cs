using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
public class Tower : NetworkBehaviour
{
    public NetworkVariable<float> projectileSpeed;
    public NetworkVariable<float> attackRange;
    public NetworkVariable<int> damage;
    public NetworkObject projectilePrefab;
    public bool canAttack;
    [SerializeField] private float attackSpeed = 0.2f;
    public NetworkObject target;

    [SerializeField]
    private bool isMoving;
    private Stats baseStats;
    private List<Stats> bonusStats = new();
    private float timer = 0f;

    [Header("Synced Variables")]
    private NetworkVariable<FixedString512Bytes> m_GameObjectName = new NetworkVariable<FixedString512Bytes>("Default Tower Name", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Stats> m_baseStats = new NetworkVariable<Stats>();
    [SerializeField]
    public NetworkVariable<Vector3> m_Position = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField]
    public NetworkVariable<FixedString512Bytes> m_TowerName = new NetworkVariable<FixedString512Bytes>("Default Tower Name", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Components")]
    protected SpriteRenderer m_Renderer;

    public Stats BaseStats => baseStats;
    public Stats BonusStats
    {
        get
        {
            Stats totalBonus = new();
            foreach (Stats stats in bonusStats) totalBonus += stats;
            return totalBonus;
        }
    }
    //public Stats TotalStats
    //{
    //    get
    //    {
    //        Stats finalStats = baseStats;
    //        foreach (Stats stats in bonusStats) baseStats += stats;
    //        return finalStats;
    //    }
    //}
    private void AttackCooldown()
    {
        if (!IsServer) return;
        if (!canAttack)
        {
            timer += Time.deltaTime;
            if (timer >= attackSpeed)
            {
                timer -= attackSpeed;
                canAttack = true;
            }
        }
    }
    //[Rpc(SendTo.Server)]
    //public void AttackRpc()
    //{
    //    Vector3 enemyPos = GetClosestEnemy(out Vector3 facingVector);
    //    NetworkObject bullet = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(projectilePrefab);
    //    bullet.transform.position = transform.position;
    //    bullet.transform.localEulerAngles = facingVector;
    //}
    //private Vector3 GetClosestEnemy(out Vector3 facingVector)
    //{
    //    Vector3 result = Vector3.positiveInfinity;
    //    facingVector = Vector3.zero;
    //    foreach (EnemyBehaviour enemy in Enemies)
    //    {
    //        facingVector = transform.position - enemy.transform.position;
    //        if (result.magnitude <= facingVector.magnitude) 
    //            result = enemy.transform.position;
    //            facingVector = facingVector.normalized;
    //    }
    //    return result;
    //}
    public override void OnNetworkSpawn()
    {
        m_GameObjectName.OnValueChanged += OnGameObjectNameChangeRpc;
        m_Position.OnValueChanged += OnPositionChangedRpc;
        m_TowerName.OnValueChanged += OnTowerNameChangedRpc;


        if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            // Set text color to green for the local player's tower
            GetComponentInChildren<TextMeshPro>().color = Color.green;
        }

        m_TowerName.Value = GameObject.Find($"Player_{OwnerClientId}").GetComponent<PlayerClientController>().playerName.Value.ToString();


        // Late join safety
        OnGameObjectNameChangeRpc(new FixedString512Bytes(""), gameObject.name.Replace("(Clone)", $"_{OwnerClientId}"));
        OnPositionChangedRpc(Vector3.zero, m_Position.Value);
        OnTowerNameChangedRpc(new FixedString512Bytes(""), GameObject.Find($"Player_{OwnerClientId}").GetComponent<PlayerClientController>().playerName.Value.ToString());
    }


    public void StartMovementRpc()
    {
        Debug.Log($"StartMovement | Local={NetworkManager.Singleton.LocalClientId} " +
              $"Owner={OwnerClientId} IsOwner={IsOwner}");

        if (!IsOwner) return;
        isMoving = true;
    }

    public void UpdateCharacterPositionRpc(Vector3 targetPosition)
    {
        m_Position.Value = targetPosition;
        transform.position = m_Position.Value;
    }

    [Rpc(SendTo.Owner)]
    private void ToggleCharacterMovementRpc(RpcParams rpcParams = default)
    {
        if (!IsOwner) return;

        // Check raycast for overlapping characters
        Collider2D[] hitAll = Physics2D.OverlapPointAll(transform.position);

        foreach(Collider2D hit in hitAll )
        {
            if (hit.gameObject != gameObject) 
            {
                Debug.Log($"Overlapping with object: {hit.gameObject.name}");
                return;
            }
        }

        if (isMoving == true)
        {
            isMoving = false;
        }
    }    

    public void CharacterMovementState()
    {
        if (!IsOwner && !isMoving)
        {
            transform.position = m_Position.Value;
        }

        if (!isMoving)
            return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;

        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(mousePos);
        cursorPos.z = 0;

        UpdateCharacterPositionRpc(cursorPos);
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsOwner)
        {
            // Disable character movement if active
            ToggleCharacterMovementRpc();
        }

        CharacterMovementState();
        transform.position = m_Position.Value;

        AttackCooldown();
    }

    protected void OnMouseDown()
    {
        if (IsOwner)
        {
            // Activate UI element
            UIManager.Instance.TowerControlPanel.SetActive(true);
            UIManager.Instance.seletedTower = gameObject;
        }
        else
        {
            UIManager.Instance.TowerControlPanel.SetActive(false);
            UIManager.Instance.seletedTower = null;
        }
    }

    [Rpc(SendTo.Server)]
    protected void OnPositionChangedRpc(Vector3 oldValue,  Vector3 newValue)
    {
        transform.position = newValue;
    }

    [Rpc(SendTo.Everyone)]
    protected void OnTowerNameChangedRpc(FixedString512Bytes oldValue, FixedString512Bytes newValue)
    {
        GetComponentInChildren<TextMeshPro>().text = newValue.ToString();
    }

    [Rpc(SendTo.Everyone)]
    protected void OnGameObjectNameChangeRpc(FixedString512Bytes oldValue, FixedString512Bytes newValue)
    {
        gameObject.name = newValue.ToString();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange.Value);
    }
}
