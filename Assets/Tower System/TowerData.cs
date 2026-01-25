using UnityEngine;

public struct TowerData
{
    private string name;
    private Stats stats;
    public string Name{ get { return name; } }
    public Stats Stats{ get { return stats; } }

    public TowerData(string name, int health, int damage, float range, float attackSpeed)
    {
        this.name = name;
        stats = new Stats(health, damage, range, attackSpeed);
    }
}
