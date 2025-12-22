using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/**
 * a player movement controller to get input from the player and move the character accordingly
 * 
 * note that only one player movement controller can exist at a time
 */
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference jump;

    private MovementBehavior player;
    private static float previousXPosition = 0f;
    private static PlayerMovementController singleton;
    public static UnityEvent<float> cameraMovedCallback = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (singleton is not null)
            throw new System.InvalidOperationException("Only one Player Controller should exist at a time.");
        singleton = this;
        player = GetComponentInParent<MovementBehavior>();
        movement.action.Enable();
    }

    /**
     * ensures a new player controller can be created after this one is destroyed
     */
    private void OnDestroy()
    {
        singleton = null;
    }

    /**
     * called every physics update and gets the state of the movement input to move the player
     */
    void FixedUpdate()
    {
        // get the screen bbox in world coordinates
        float left = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).x;
        float right = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)).x;

        // moves player and camera to enure player is on screen
        float xPosition = player.move(movement.action.ReadValue<Vector2>().x * Time.fixedDeltaTime);
        if (xPosition < left + 2 || xPosition > right - 2)
        {
            Camera.main.transform.position += new Vector3(xPosition - previousXPosition, 0, 0);
            cameraMovedCallback.Invoke(xPosition - previousXPosition);
        }
        previousXPosition = xPosition;
    }

    /**
     * called automatically by the input system when the jump input is triggered
     */
    private void OnJump()
    {
        player.jump();
    }
}
