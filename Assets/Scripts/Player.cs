using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
public class Player : Tower
{
    public List<Sprite> characterSpriteList;

    [Header("Network Variables")]
    public int m_characterSpriteIndex = 0;
    private NetworkVariable<int> characterSpriteIndex = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Vector3 localPosition;

    [SerializeField]
    private bool isMoving;

    protected override void Awake()
    {
        base.Awake();

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

    private void ToggleCharacterMovement()
    {
        if (!IsOwner) return;

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

    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Disable character movement if active
            ToggleCharacterMovement();
        }
        CharacterMovementState();
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
