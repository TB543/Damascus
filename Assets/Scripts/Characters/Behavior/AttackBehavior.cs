using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackBehavior : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private AbstractAttack[] attacks;

    private AbstractAttack currentAttack;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D hurtbox;
    private int collisionLayer;
    private HashSet<AttackBehavior> collisions = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collisionLayer = GetComponent<PlayerAttackController>() == null ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hurtbox = GetComponentInChildren<PolygonCollider2D>();
        for (int i = 0; i < attacks.Length; i++)
            attacks[i].init(i + 1, animator);
    }

    private void FixedUpdate()
    {
        foreach (AttackBehavior collision in collisions.ToArray())
            currentAttack.applyDamage(collision);
    }

    public void takeDamage(float damage)
    {
        float previousHealth = health;
        health -= damage;
        if (health <= 0 && previousHealth > 0)
            animator.SetTrigger("Death");
        else if (health > 0)
            animator.SetTrigger("Damage");
    }

    public void startAttack(int attackIndex)
    {
        if (currentAttack is null || currentAttack.endAttack())
        {
            removeHurtBox();
            attacks[attackIndex].startAttack();
            currentAttack = attacks[attackIndex];
        }
    }

    public void endAttack(int attackIndex)
    {
        if (attacks[attackIndex].endAttack() && attacks[attackIndex] == currentAttack)
        {
            removeHurtBox();
            currentAttack = null;
        }
    }

    private void endAttackAnimation()
    {
        currentAttack.endAttack(true);
        removeHurtBox();
        currentAttack = null;
    }

    private void setHurtBox()
    {
        List<Vector2> points = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, points);
        hurtbox.points = points.ToArray();
        hurtbox.enabled = true;
    }

    private void removeHurtBox()
    {
        hurtbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == collisionLayer && (!collision.isTrigger))
            collisions.Add(collision.gameObject.GetComponent<AttackBehavior>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == collisionLayer && (!collision.isTrigger))
            collisions.Remove(collision.gameObject.GetComponent<AttackBehavior>());
    }

    private void spawnProjectile()
    {
        (currentAttack as ProjectileAttack).spawnProjectile(transform);
    }
}
