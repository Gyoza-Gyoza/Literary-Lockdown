using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    private List<EnemyBehaviour> enemiesInRange = new();

    public List<EnemyBehaviour> EnemiesInRange
    { get { return enemiesInRange; } }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBehaviour enemy = collision.GetComponent<EnemyBehaviour>();
        if (enemy != null) enemiesInRange.Add(enemy);
    }
}
