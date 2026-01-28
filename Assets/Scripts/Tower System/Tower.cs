using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Globalization;
using UnityEngine.UIElements;
using Unity.Collections;

public abstract class Tower : NetworkBehaviour
{
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

    protected virtual void Awake()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Renderer.sprite = defaultSprite;
    }

    protected virtual void Update()
    {
        transform.position = m_Position.Value;
    }
}
