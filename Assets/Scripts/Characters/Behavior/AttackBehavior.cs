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
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hurtbox = GetComponent<PolygonCollider2D>();
        setLayer(gameObject.layer);
        for (int i = 0; i < attacks.Length; i++)
            attacks[i].init(i + 1, animator);
    }

    public void setLayer(int layer)
    {
        gameObject.layer = layer;
        collisionLayer = layer == LayerMask.NameToLayer("Enemy") ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");
        collisions.Clear();
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
        if ((attackIndex >= 0 && attackIndex < attacks.Length) && (currentAttack is null || currentAttack.endAttack()))
        {
            removeHurtBox();
            attacks[attackIndex].startAttack();
            currentAttack = attacks[attackIndex];
        }
    }

    public void endAttack(int attackIndex)
    {
        if ((attackIndex >= 0 && attackIndex < attacks.Length) && (attacks[attackIndex].endAttack() && attacks[attackIndex] == currentAttack))
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
        collisions.Clear();
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
