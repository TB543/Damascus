using UnityEngine;

/**
 * an attack that spawns a projectile
 * damage is applied by the projectile itself
 * 
 * see AbtractAttack for more information
 */
[CreateAssetMenu(fileName = "ProjectileAttack", menuName = "Scriptable Objects/ProjectileAttack")]
public class ProjectileAttack : AbstractAttack
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float initialVelocity;
    [SerializeField] private float maxDistance;
    private bool running = false;

    public override void startAttack()
    {
        animator.SetTrigger("Attack" + attackNum);
        running = true;
    }

    public override void applyDamage(AttackBehavior enemy)
    {
        // damage is handled in the projectile
    }

    public override bool endAttack(bool force = false)
    {
        if (force)
            running = false;
        return !running;
    }

    /**
     * spawns the projectile at the given transform
     * 
     * @param transform the hero that spawned the projectile
     */
    public GameObject spawnProjectile(AttackBehavior parent)
    {
        GameObject projectile = Instantiate(projectilePrefab, parent.transform);
        projectile.transform.parent = null;
        projectile.GetComponent<SpriteRenderer>().enabled = parent.GetComponent<SpriteRenderer>().enabled;
        projectile.GetComponent<ProjectileBehavior>().init(damage, maxDistance, initialVelocity, parent);
        return projectile;
    }
}
