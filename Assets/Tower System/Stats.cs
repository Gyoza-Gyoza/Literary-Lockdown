using UnityEngine;

public struct Stats
{
    private int health, damage;
    private float range, attackSpeed;
    public int Health { get { return health; } }
    public int Damage { get { return damage; } }
    public float Range { get { return range; } }
    public float AttackSpeed { get { return attackSpeed; } }

    public Stats(int health, int damage, float range, float attackSpeed)
    {
        this.health = health;
        this.damage = damage;
        this.range = range;
        this.attackSpeed = attackSpeed;
    }
    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats(a.health += b.health,
        a.damage += b.damage,
        a.range += b.range,
        a.attackSpeed += b.attackSpeed);
    }
}
