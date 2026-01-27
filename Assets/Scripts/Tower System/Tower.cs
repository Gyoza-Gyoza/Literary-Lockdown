using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
public abstract class Tower : NetworkBehaviour
{
    private Stats baseStats;
    private List<Stats> bonusStats = new();

    public Sprite defaultSprite;

    [Header("Synced Variables")]
    private NetworkVariable<Stats> m_baseStats = new NetworkVariable<Stats>();
    //private NetworkVariable<string> m_characterSprite = new NetworkVariable<string>();
    private NetworkVariable<Vector3> m_Position = new NetworkVariable<Vector3>();

    [Header("Components")]
    private SpriteRenderer m_Renderer;

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
    public Stats TotalStats => BaseStats + BonusStats;
    private void Awake()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Renderer.sprite = defaultSprite;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            //Do default action here
        }
    }

    [Rpc(SendTo.Server)]
    private void SetSpriteRpc(RpcParams rpcParams = default)
    {

    }

    private void Update()
    {
        transform.position = m_Position.Value;


    }
}
