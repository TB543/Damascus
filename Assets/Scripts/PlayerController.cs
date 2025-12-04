using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference keyboard;
    [SerializeField] private InputActionReference lightAttack;
    [SerializeField] private InputActionReference heavyAttack;
    public static UnityEvent<float> cameraMovedCallback = new UnityEvent<float>();

    // Update is called once per frame
    void Update()
    {
        // gets velocity
        Vector2 velocity = keyboard.action.ReadValue<Vector2>() * Time.deltaTime * 5;
        GetComponent<Animator>().SetInteger("Velocity", (int)Mathf.Ceil(velocity.magnitude));

        // adjusts position and rotation
        transform.position += new Vector3(velocity.x, velocity.y);
        Vector3 oldScale = transform.localScale;
        if (velocity.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(velocity.x) * Mathf.Abs(oldScale.x), oldScale.y, oldScale.z);

        // get the screen bbox in world coordinates
        Camera cam = Camera.main;
        float left = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)).x;
        float right = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane)).x;

        // moves camera to follow player
        if (transform.position.x < left + 2 || transform.position.x > right - 2)
        {
            cam.transform.position = new Vector3(cam.transform.position.x + velocity.x, cam.transform.position.y, cam.transform.position.z);
            cameraMovedCallback.Invoke(velocity.x);
        }
    }

    private void OnLightAttack()
    {
        GetComponent<Animator>().SetTrigger("LightAttack");
    }

    private void OnHeavyAttack()
    {
        GetComponent<Animator>().SetTrigger("HeavyAttack");
    }
}
