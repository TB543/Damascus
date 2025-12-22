using UnityEngine;

/**
 * handles the movement behavior of a character
 * communicates with the animator to process movement animations
 * 
 * game objects with this script should also be equipped with an movement controller to handle hero input
 */
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

    /*
     * adjusts animations based on physics
     */
    private void FixedUpdate()
    {
        animator.SetInteger("XVelocity", (int)body.linearVelocityX);
        animator.SetInteger("YVelocity", (int)(body.linearVelocityY * 1000));
    }

    /**
     * moves the player horizontally based on the given velocity.
     * must be continuously called every physics update to maintain movement
     * 
     * @return the new x position of the hero
     */
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
        return body.position.x;
    }

    /**
     * called when the hero jumps. only called once when the jump is initiated
     */
    public void jump()
    {
        if (currentJumps < numJumps - 1)
        {
            body.linearVelocityY = jumpForce;
            currentJumps++;
        }
    }
}
