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
    private NetworkVariable<int> characterSpriteIndex = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

        if (IsOwner)
        {
            UIManager.Instance.CharacterSelect_UI.SetActive(true);
        }

        // Apply immediately for late joiners
        if (characterSpriteIndex.Value != -1)
            OnSpriteChanged(-1, characterSpriteIndex.Value);
    }

    [Rpc(SendTo.Server)]
    public void SetSpriteRpc(RpcParams rpcParams = default)
    {
        characterSpriteIndex.Value = m_characterSpriteIndex;

        if (IsOwner)
        {
            // Enable Character movement for placement
            isMoving = true;
        }
    }

    [Rpc(SendTo.Server)]
    public void UpdateCharacterPositionRpc(RpcParams rpcParams = default)
    {
        m_Position.Value = localPosition;
        transform.position = localPosition;
    }

    private void ToggleCharacterMovement()
    {
        if (isMoving == true)
        {
            isMoving = false;
        }
    }    

    public void CharacterMovementState()
    {
        switch(isMoving)
        {
            case false:
                // No movement
                transform.position = m_Position.Value;
                break;

            case true:
                // Follow character based on movement of cursor
                Vector3 mousePostion = Input.mousePosition;

                Vector3 cursorPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePostion.x, mousePostion.y, 10f));
                cursorPos.z = 0;

                localPosition = cursorPos;

                // Update the server
                UpdateCharacterPositionRpc();
                break;
        }
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
        m_Renderer.sprite = characterSpriteList[newValue];
    }
}
