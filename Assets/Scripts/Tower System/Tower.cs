using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tower : NetworkBehaviour
{
    public NetworkVariable<float> projectileSpeed;
    public NetworkVariable<float> attackRange;
    public NetworkVariable<float> damage;
    public NetworkObject projectilePrefab;
    public bool canAttack = false;
    [SerializeField] private float attackSpeed = 0.2f;
    public NetworkObject target;

    public targetStyle towerTargetStyle = targetStyle.Closest;
    public enum targetStyle {Furthest, Closest}

    [SerializeField]
    private bool isMoving;
    [SerializeField]
    private string towerType;
    private Stats baseStats;
    private List<Stats> bonusStats = new();
    private float timer = 0f;
    private float initialXScale;

    [Header("Synced Variables")]
    private NetworkVariable<FixedString512Bytes> m_GameObjectName = new NetworkVariable<FixedString512Bytes>("Default Tower Name", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Stats> m_baseStats = new NetworkVariable<Stats>();
    [SerializeField]
    public NetworkVariable<Vector3> m_Position = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField]
    public NetworkVariable<FixedString512Bytes> m_TowerName = new NetworkVariable<FixedString512Bytes>("Default Tower Name", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Components")]
    //protected SpriteRenderer m_Renderer;
    private Animator animator;
    [SerializeField] protected GameObject rangeIndicator;


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

    private void Awake()
    {
        initialXScale = transform.localScale.x;
    }

    private void AttackCooldown()
    {
        Debug.Log("Cooling down");
        if (!IsServer) return;
        if (ObjectivesManager.Instance.gameEnded.Value) return;


        if (!canAttack)
        {
            float interval = 1f / attackSpeed;

            if (timer < interval)
            {
                timer += Time.fixedDeltaTime;
            }
            

            if (timer >= interval && target != null)
            {
                Debug.Log("Attacking");
                animator.SetTrigger("CanAttack");
                timer -= interval;
                
            }
        }
    }
    public void InitializeStats(TowerData chosenTower)
    {
        animator = GetComponent<Animator>();
        this.damage.Value = chosenTower.Damage;
        this.attackSpeed = chosenTower.AttackSpeed;
    }
    public virtual void Attack()
    {
        if (target == null) return;
        canAttack = true;
        PlayAttackingSFX();

        bool isFlipped = transform.position.x < target.transform.position.x; 
        transform.localScale = new Vector3(isFlipped? initialXScale : -initialXScale, transform.localScale.y, transform.localScale.z);

        
        //Should trigger animation instead
        NetworkObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = transform.position;

        Vector2 direction = target.transform.position - transform.position;
        direction.Normalize();

        projectile.transform.rotation = Quaternion.FromToRotation(Vector2.up, direction);

        var bullet = projectile.GetComponentInChildren<Bullet>();
        if (bullet != null)
        {
            bullet.speed.Value = projectileSpeed.Value;
            bullet.damage.Value = damage.Value;
        }
        projectile.Spawn();
        canAttack = false;
    }
    public void PlayAttackingSFX()
    {
        switch (towerType)
        {
            case "Wolf":
                AudioManager.PlayWolfAttackSFX();
                break;
            case "Rapu":
                AudioManager.PlayRapAttackSFX();
                break;
            default:
                break;
        }
    }
    public override void OnNetworkSpawn()
    {
        m_GameObjectName.OnValueChanged += OnGameObjectNameChangeRpc;
        m_Position.OnValueChanged += OnPositionChangedRpc;
        m_TowerName.OnValueChanged += OnTowerNameChangedRpc;
        towerType = GetTowerData(this.gameObject.name);

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
        DisplayRange();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // Find the tower 's index in the tower list and remove it
        TowerManager.Instance.towerList.Remove(this);
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

    public void DisplayRange()
    {
        rangeIndicator.SetActive(true);
        rangeIndicator.transform.localScale = new Vector3(attackRange.Value * 2, attackRange.Value* 2, attackRange.Value*2);
    }

    public void HideRange()
    {
        rangeIndicator.SetActive(false);
        //rangeIndicator.transform.localScale = new Vector3(attackRange.Value, attackRange.Value, attackRange.Value);
    }

    public string GetTowerData(string thisTower)
    {
        if(thisTower.Length >= 4)
        {
            string firstFour = thisTower.Substring(0, 4);
            thisTower = firstFour;
        }
        else
        {
            Debug.Log("Name has less than 4 characters");
        }  
        return thisTower;
    }

    [Rpc(SendTo.Owner)]
    private void ToggleCharacterMovementRpc(RpcParams rpcParams = default)
    {
        if (!IsOwner) return;

        // Check for tilemap pathing
        GridLayout gridLayout = GameObject.FindWithTag("TileMap").GetComponent<GridLayout>();
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = gridLayout.WorldToCell(cursorPos);

        TowerDefenseTiles tile = gridLayout.GetComponentInChildren<Tilemap>().GetTile(cellPosition) as TowerDefenseTiles;

        if (tile != null && !tile.towerFriendly) return;  

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
        if (IsOwner && ObjectivesManager.Instance.isGameStart() == false)
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

    [Rpc(SendTo.Server)]
    public void DestroyTowerRpc(RpcParams rpcParams = default)
    {
        // Prevent unauthorized clients from despawning another player's tower
        if (rpcParams.Receive.SenderClientId != OwnerClientId) return;


        GetComponent<NetworkObject>().Despawn();
    }
}
