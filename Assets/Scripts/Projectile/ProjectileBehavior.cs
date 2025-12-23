using UnityEngine;

/*
 * handles projectile movement, collision detection, and damage application
 */
public class ProjectileBehavior : MonoBehaviour
{
    private float damage;
    private float maxDistance;
    private AttackBehavior parent;
    private int collisionLayer;
    private float startingPosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;

    /*
     * gets the parameters from the projectile from the attack behavior/attack
     */
    public void init(float damage, float maxDistance, float initialVelocity, AttackBehavior parent)
    {
        this.damage = damage;
        this.maxDistance = maxDistance;
        this.parent = parent;
        collisionLayer = parent.gameObject.layer == LayerMask.NameToLayer("Enemy") ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");
        startingPosition = transform.position.x;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        body.linearVelocityX = initialVelocity * Mathf.Sign(transform.localScale.x);
    }

    /*
     * updates the position of the projectile every physics frame
     */
    void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x - startingPosition) >= maxDistance)
        {
            animator.SetTrigger("Explode");
            body.linearVelocityX = 0;
        }
    }

    /*
     * handles collisions
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == collisionLayer && (!collision.isTrigger) && spriteRenderer.enabled)
        {
            boxCollider.enabled = false;
            animator.SetTrigger("Explode");
            body.linearVelocityX = 0;
            collision.GetComponent<AttackBehavior>().takeDamage(damage);
        }
    }

    /*
     * called by the animator when the animation has finished
     */
    public void destroy()
    {
        Destroy(gameObject);
        parent.removeProjectile(gameObject);
    }
}
