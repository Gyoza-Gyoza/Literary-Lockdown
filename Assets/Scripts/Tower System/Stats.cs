using Unity.Netcode;
using UnityEngine;

public struct Stats : INetworkSerializable
{
    public int Damage;
    public float Range;
    public float AttackSpeed;
    public Stats(int damage = 0, float range = 0f, float attackSpeed = 0f)
    {
        Damage = damage;
        Range = range;
        AttackSpeed = attackSpeed;
    }
    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats(a.Damage + b.Damage,
        a.Range + b.Range,
        a.AttackSpeed + b.AttackSpeed);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Damage);
        serializer.SerializeValue(ref Range);
        serializer.SerializeValue(ref AttackSpeed);
    }
}
