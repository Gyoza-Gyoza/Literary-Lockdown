using UnityEngine;

public struct TowerData
{
    private string name;
    private Stats stats;
    private Sprite sprite;
    public string Name{ get { return name; } }
    public Stats Stats{ get { return stats; } }
    public Sprite Sprite{ get { return sprite; } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">Name of the towerw</param>
    /// <param name="spritePath">Path to find the sprite, defaults to the name if it's empty</param>
    /// <param name="damage">Damage of the tower</param>
    /// <param name="range">Range of the tower</param>
    /// <param name="attackSpeed">Attack speed of the tower</param>
    public TowerData(string name, string spritePath, int damage, float range, float attackSpeed)
    {
        this.name = name;
        sprite = Resources.Load<Sprite>($"Sprites/{spritePath}");
        stats = new Stats(damage, range, attackSpeed);
    }
}
