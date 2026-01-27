using UnityEngine;

public struct Stats
{
    private int damage;
    private float range, attackSpeed;
    public int Damage { get { return damage; } }
    public float Range { get { return range; } }
    public float AttackSpeed { get { return attackSpeed; } }

    public Stats(int damage = 0, float range = 0f, float attackSpeed = 0f)
    {
        this.damage = damage;
        this.range = range;
        this.attackSpeed = attackSpeed;
    }
    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats(a.damage += b.damage,
        a.range += b.range,
        a.attackSpeed += b.attackSpeed);
    }
}
