using UnityEngine;
using Unity.Netcode;
public class FrogPrince : Tower
{
    protected override void AttackCooldown()
    {
        //Debug.Log("Cooling down");
        if (!IsServer) return;
        if (ObjectivesManager.Instance.gameEnded.Value) return;


        if (!canAttack)
        {
            float interval = 1f / attackSpeed;

            if (timer <= interval)
            {
                timer += Time.deltaTime;
            }
            

            if (timer >= interval && target != null)
            {
                //Debug.Log("Attacking");
                animator.SetTrigger("CanAttack");
                timer -= interval;
                PlayAttackingSFX();
            }
        }
    }
    public override void Attack()
    {
        if (target == null) return;
        canAttack = true;

        bool isFlipped = transform.position.x < target.transform.position.x; 
        transform.localScale = new Vector3(isFlipped? initialXScale : -initialXScale, transform.localScale.y, transform.localScale.z);

        
        //Should trigger animation instead
        NetworkObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = transform.position;

        Vector2 direction = target.transform.position - transform.position;
        direction.Normalize();

        projectile.transform.rotation = Quaternion.FromToRotation(Vector2.up, direction);

        var bullet = projectile.GetComponentInChildren<Bullet>();
        if (bullet != null)
        {
            bullet.speed.Value = projectileSpeed.Value;
            bullet.damage.Value = damage.Value;
        }
        projectile.Spawn();
        canAttack = false;
    }
}
