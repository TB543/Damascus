using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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

    private void OnDestroy()
    {
        singleton = null;
    }

    void FixedUpdate()
    {
        // get the screen bbox in world coordinates
        Camera cam = Camera.main;
        float left = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)).x;
        float right = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane)).x;

        // moves player and camera to enure player is on screen
        float xPosition = player.move(movement.action.ReadValue<Vector2>().x * Time.fixedDeltaTime);
        if (xPosition < left + 2 || xPosition > right - 2)
        {
            cam.transform.position += new Vector3(xPosition - previousXPosition, 0, 0);
            cameraMovedCallback.Invoke(xPosition - previousXPosition);
        }
        previousXPosition = xPosition;
    }

    private void OnJump()
    {
        player.jump();
    }
}
