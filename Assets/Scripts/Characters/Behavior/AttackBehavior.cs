using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * handles the attack behavior of a character, including health, stamina, and attacks
 * communicates with the animator and attacks to process attack states
 * 
 * game objects with this script should also be equipped with an attack controller to handle hero input
 */
public class AttackBehavior : MonoBehaviour
{
    [SerializeField] HeroClasses heroClass;
    [SerializeField] HeroTypes heroType;
    [SerializeField] private float health;
    [SerializeField] private float stamina;
    [SerializeField] private AbstractAttack[] attacks;

    private AbstractAttack currentAttack;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D hurtbox;
    private int collisionLayer;
    private HashSet<AttackBehavior> collisions = new();

    public HeroClasses HeroClass => heroClass;
    public HeroTypes HeroType => heroType;
    public float Health => health;
    public float Stamina => stamina;
    public AbstractAttack[] Attacks => attacks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hurtbox = GetComponent<PolygonCollider2D>();
        setLayer(gameObject.layer);
        for (int i = 0; i < attacks.Length; i++)
            attacks[i] = attacks[i].init(i + 1, animator);
    }

    /*
     * ensures instance of scriptable objects are destroyed 
     */
    private void OnDestroy()
    {
        foreach (AbstractAttack attack in attacks)
            Destroy(attack);
    }

    /**
     * sets the layer of the game object and updates the collision layer accordingly
    */
    public void setLayer(int layer)
    {
        gameObject.layer = layer;
        collisionLayer = layer == LayerMask.NameToLayer("Enemy") ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");
        collisions.Clear();
    }

    /**
     * applies damage to all current collisions during the physics update
     */
    private void FixedUpdate()
    {
        if (currentAttack is not null)
            foreach (AttackBehavior collision in collisions.ToArray())
                currentAttack.applyDamage(collision);
    }

    /**
     * handles when the hero is attacked by another hero
    */
    public void takeDamage(float damage)
    {
        float previousHealth = health;
        health -= damage;
        if (health <= 0 && previousHealth > 0)
            animator.SetTrigger("Death");
        else if (health > 0)
            animator.SetTrigger("Damage");
    }

    /**
     * starts an attack on one of the following conditions:
     *  - no attack is being performed
     *  - the current attack can be ended
     */
    public void startAttack(int attackIndex)
    {
        if ((attackIndex >= 0 && attackIndex < attacks.Length) && (currentAttack is null || currentAttack.endAttack()))
        {
            removeHurtBox();
            attacks[attackIndex].startAttack();
            currentAttack = attacks[attackIndex];
        }
    }

    /**
     * attempts to end the current attack. called by player input
     */
    public void endAttack(int attackIndex)
    {
        if ((attackIndex >= 0 && attackIndex < attacks.Length) && (attacks[attackIndex].endAttack() && attacks[attackIndex] == currentAttack))
        {
            removeHurtBox();
            currentAttack = null;
        }
    }

    /**
     * forces the attack animation to end. called by the animator
     */
    private void endAttackAnimation()
    {
        currentAttack.endAttack(true);
        removeHurtBox();
        currentAttack = null;
    }

    /**
     * called by the animator to set the hurtbox on a given frame
     */
    private void setHurtBox()
    {
        List<Vector2> points = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, points);
        hurtbox.points = points.ToArray();
        hurtbox.enabled = true;
    }

    /**
     * removes the hurtbox, called by either the animator when the attack frame ends or when the attack is ended
     */
    private void removeHurtBox()
    {
        hurtbox.enabled = false;
        collisions.Clear();
    }

    /**
     * keeps track of all collisions in the hurtbox
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == collisionLayer && (!collision.isTrigger))
            collisions.Add(collision.gameObject.GetComponent<AttackBehavior>());
    }

    /**
     * removes collisions that exit the hurtbox
     */
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == collisionLayer && (!collision.isTrigger))
            collisions.Remove(collision.gameObject.GetComponent<AttackBehavior>());
    }

    /**
     * called by the animator to spawn a projectile during a projectile attack
     */
    private void spawnProjectile()
    {
        (currentAttack as ProjectileAttack).spawnProjectile(transform);
    }
}
