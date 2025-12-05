using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterBehavior : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 300f;
    [SerializeField] private float jumpForce = 25f;
    [SerializeField] private float numJumps = 2f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float lightDamage = 10f;
    [SerializeField] private float heavyDamage = 20f;

    private float currentJumps = 0f;
    private float previous_pos = 0f;
    private float current_damage = 0f;
    private HashSet<GameObject> enemiesHit = new HashSet<GameObject>();

    private Animator animator;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D hurtbox;

    [HideInInspector]
    public float xMovement = 0f;
    public static UnityEvent<float> cameraMovedCallback = new UnityEvent<float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hurtbox = GetComponentInChildren<PolygonCollider2D>();
    }

    void FixedUpdate()
    {
        // adjusts position and rotation
        float velocity = xMovement * Time.fixedDeltaTime * moveSpeed;
        body.linearVelocityX = velocity;
        Vector3 oldScale = transform.localScale;
        if (velocity != 0)
            transform.localScale = new Vector3(Mathf.Sign(velocity) * Mathf.Abs(oldScale.x), oldScale.y, oldScale.z);

        // resets jumps if player has touched the ground
        if (body.IsTouchingLayers(LayerMask.GetMask("Background", "Enemy")))
            currentJumps = 0;

        // get the screen bbox in world coordinates
        Camera cam = Camera.main;
        float left = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)).x;
        float right = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane)).x;

        // moves camera to follow player
        if (body.position.x < left + 2 || body.position.x > right - 2)
        {
            cam.transform.position += new Vector3(body.position.x - previous_pos, 0, 0);
            cameraMovedCallback.Invoke(body.position.x - previous_pos);
        }
        previous_pos = body.position.x;

        // updates animator
        animator.SetInteger("XVelocity", (int)velocity);
        animator.SetInteger("YVelocity", (int)(body.linearVelocityY * 1000));
    }

    public void jump()
    {
        if (currentJumps < numJumps - 1)
        {
            body.linearVelocityY = jumpForce;
            currentJumps++;
        }
    }

    public void lightAttack()
    {
        if (current_damage == 0)
        {
            animator.SetTrigger("LightAttack");
            current_damage = lightDamage;
        }
    }

    public void heavyAttack()
    {
        if (current_damage == 0)
        {
            animator.SetTrigger("HeavyAttack");
            current_damage = heavyDamage;
        }
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

    private void setHurtBox()
    {
        List<Vector2> points = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, points);
        hurtbox.points = points.ToArray();
        hurtbox.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && !enemiesHit.Contains(collider.gameObject))
        {
            enemiesHit.Add(collider.gameObject);
            collider.gameObject.GetComponent<CharacterBehavior>().takeDamage(current_damage);
        }
    }

    private void removeHurtBox()
    {
        hurtbox.enabled = false;
        current_damage = 0f;
        enemiesHit.Clear();
    }
}
