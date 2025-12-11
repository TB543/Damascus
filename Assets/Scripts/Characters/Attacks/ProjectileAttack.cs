using UnityEngine;

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

    public void spawnProjectile(Transform transform)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform);
        Vector3 worldPosition = projectile.transform.position;
        Quaternion worldRotation = projectile.transform.rotation;
        projectile.transform.parent = null;
        projectile.transform.position = worldPosition;
        projectile.transform.rotation = worldRotation;
        projectile.GetComponent<ProjectileBehavior>().init(damage, maxDistance, initialVelocity, transform.gameObject.layer);
    }
}
