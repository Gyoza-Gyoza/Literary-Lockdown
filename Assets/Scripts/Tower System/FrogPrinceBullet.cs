using System.Collections;
using UnityEngine;

public class FrogPrinceBullet : Bullet
{
    [SerializeField] private float slowDuration; 
    [Range(0,1)][SerializeField] private float slowAmount;
    protected override void OnDamage(EnemyBehaviour enemy)
    {
        enemy.SlowDownRPC(slowAmount, slowDuration);
    }
}
