using UnityEngine;

public class MovementBehavior : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float numJumps;

    private Animator animator;
    private Rigidbody2D body;
    private float currentJumps = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    public float move(float xVelocity)
    {
        // adjusts position and rotation
        float velocity = xVelocity * moveSpeed;
        body.linearVelocityX = velocity;
        Vector3 oldScale = transform.localScale;
        if (velocity != 0)
            transform.localScale = new Vector3(Mathf.Sign(velocity) * Mathf.Abs(oldScale.x), oldScale.y, oldScale.z);

        // resets jumps if character has touched the ground
        if (body.IsTouchingLayers(LayerMask.GetMask("Background", "Enemy")))
            currentJumps = 0;

        // updates animator
        animator.SetInteger("XVelocity", (int)velocity);
        animator.SetInteger("YVelocity", (int)(body.linearVelocityY * 1000));
        return body.position.x;
    }

    public void jump()
    {
        if (currentJumps < numJumps - 1)
        {
            body.linearVelocityY = jumpForce;
            currentJumps++;
        }
    }
}
