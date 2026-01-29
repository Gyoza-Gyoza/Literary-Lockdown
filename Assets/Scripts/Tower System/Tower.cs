using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
public class Tower : NetworkBehaviour
{
    public List<Sprite> characterSpriteList;

    [Header("Network Variables")]
    public int m_characterSpriteIndex = 0;
    private NetworkVariable<int> characterSpriteIndex = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Vector3 localPosition;

    [SerializeField]
    private bool isMoving;
    private Stats baseStats;
    private List<Stats> bonusStats = new();

    public Sprite defaultSprite;

    [Header("Synced Variables")]
    private NetworkVariable<Stats> m_baseStats = new NetworkVariable<Stats>();
    [SerializeField]
    protected NetworkVariable<Vector3> m_Position = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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
    public Stats TotalStats
    {
        get
        {
            Stats finalStats = baseStats;
            foreach (Stats stats in bonusStats) baseStats += stats;
            return finalStats;
        }
    }
    protected void Awake()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Renderer.sprite = defaultSprite;
        isMoving = false;
    }

    public override void OnNetworkSpawn()
    {
        characterSpriteIndex.OnValueChanged += OnSpriteChanged;
        m_Position.OnValueChanged += OnPositionChangedRpc;

        if (IsOwner)
        {
            UIManager.Instance.CharacterSelect_UI.SetActive(true);
        }

        OnSpriteChanged(-1, characterSpriteIndex.Value);

        // Late join safety
        OnPositionChangedRpc(Vector3.zero, m_Position.Value);
    }

    [Rpc(SendTo.Owner)]
    public void SetSpriteRpc(int value, RpcParams rpcParams = default)
    {
        characterSpriteIndex.Value = value;
        m_characterSpriteIndex = characterSpriteIndex.Value;
        m_Renderer.sprite = characterSpriteList[m_characterSpriteIndex];

        StartMovement();
    }

    public void StartMovement()
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
        if (Input.GetMouseButtonDown(0))
        {
            // Disable character movement if active
            ToggleCharacterMovementRpc();
        }
        CharacterMovementState();
        transform.position = m_Position.Value;
    }

    protected void OnSpriteChanged(int oldValue, int newValue)
    {
        m_characterSpriteIndex = newValue;
        m_Renderer.sprite = characterSpriteList[m_characterSpriteIndex];
    }

    [Rpc(SendTo.Server)]
    protected void OnPositionChangedRpc(Vector3 oldValue,  Vector3 newValue)
    {
        transform.position = newValue;
    }
}
