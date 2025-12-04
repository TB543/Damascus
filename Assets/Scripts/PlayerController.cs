using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference lightAttack;
    [SerializeField] private InputActionReference heavyAttack;
    [SerializeField] private float moveSpeed = 300f;
    [SerializeField] private float jumpForce = 25f;
    [SerializeField] private float numJumps = 2f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float lightDamage = 10f;
    [SerializeField] private float heavyDamage = 20f;
    private float currentJumps = 0f;
    private float previous_pos = 0f;
    private float current_damage = 0f;
    public static UnityEvent<float> cameraMovedCallback = new UnityEvent<float>();

    void FixedUpdate()
    {
        // adjusts position and rotation
        float velocity = movement.action.ReadValue<Vector2>().x * Time.fixedDeltaTime * moveSpeed;
        Rigidbody2D body = GetComponent<Rigidbody2D>();
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
        Animator animatior = GetComponent<Animator>();
        animatior.SetInteger("XVelocity", (int)velocity);
        animatior.SetInteger("YVelocity", (int)(body.linearVelocityY * 1000));
    }

    private void OnJump()
    {
        if (currentJumps < numJumps - 1)
        {
            GetComponent<Rigidbody2D>().linearVelocityY = jumpForce;
            currentJumps++;
        }
    }

    private void OnLightAttack()
    {
        if (current_damage == 0)
        {
            GetComponent<Animator>().SetTrigger("LightAttack");
            current_damage = lightDamage;
        }
    }

    private void OnHeavyAttack()
    {
        if (current_damage == 0)
        {
            GetComponent<Animator>().SetTrigger("HeavyAttack");
            current_damage = heavyDamage;
        }
    }

    private void checkAttack()
    {
        // sets hurtbox
        List<Vector2> points = new List<Vector2>();
        GetComponent<SpriteRenderer>().sprite.GetPhysicsShape(0, points);
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
        collider.points = points.ToArray();

        // checks for hits
        if (collider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
            print(current_damage);  // todo damage

        // ends attack
        current_damage = 0;
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        Animator animatior = GetComponent<Animator>();
        if (health <= 0)
            animatior.SetTrigger("Death");
        else
            animatior.SetTrigger("Damage");
    }
}
