using UnityEngine;

[System.Serializable]
public struct TowerData
{
    private string name;
    private string description;
    private float damage, attackSpeed; 
    private float damagePerLevel, attackSpeedPerLevel; 
    private Sprite sprite;
    private int upgradeCost;
    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public float Damage { get { return damage; } }
    public float DamagePerLevel { get { return damagePerLevel; } }
    public float AttackSpeed { get { return attackSpeed; } }
    public float AttackSpeedPerLevel { get { return attackSpeedPerLevel; } }
    public Sprite Sprite { get { return sprite; } }
    public int UpgradeCost { get { return upgradeCost; } }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">Name of the towerw</param>
    /// <param name="spritePath">Path to find the sprite, defaults to the name if it's empty</param>
    /// <param name="damage">Damage of the tower</param>
    /// <param name="range">Range of the tower</param>
    /// <param name="attackSpeed">Attack speed of the tower</param>
    public TowerData(string name, string description, string spritePath, string damage, string attackSpeed, int upgradeCost)
    {
        this.name = name;
        this.description = description; 
        sprite = Resources.Load<Sprite>($"Sprites/{spritePath}");
        string[] damageArray = damage.Split('+');
        this.damage = float.Parse(damageArray[0]);
        this.damagePerLevel = float.Parse(damageArray[1]);
        string[] attackSpeedArray = attackSpeed.Split('+');
        this.attackSpeed = float.Parse(attackSpeedArray[0]);
        this.attackSpeedPerLevel = float.Parse(attackSpeedArray[1]);
        this.upgradeCost = upgradeCost;
        Debug.Log(name + "'s speed is stored as " + this.attackSpeed + " with increments as " + this.attackSpeedPerLevel + ". Input String is " + attackSpeed);
    }
}