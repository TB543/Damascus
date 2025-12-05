using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference lightAttack;
    [SerializeField] private InputActionReference heavyAttack;
    private CharacterBehavior characterBehavior;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterBehavior = GetComponent<CharacterBehavior>();
    }

    void FixedUpdate()
    {
        characterBehavior.xMovement = movement.action.ReadValue<Vector2>().x;
    }

    private void OnJump()
    {
        characterBehavior.jump();
    }

    private void OnLightAttack()
    {
        characterBehavior.lightAttack();
    }

    private void OnHeavyAttack()
    {
        characterBehavior.heavyAttack();
    }
}
