using System;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * connects the player input to the attack behavior
 */
public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private InputActionReference[] attacks;

    private AttackBehavior player;
    private int oldLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oldLayer = transform.parent.gameObject.layer;
        player = GetComponentInParent<AttackBehavior>();
        transform.parent.gameObject.GetComponent<AttackBehavior>().setLayer(LayerMask.NameToLayer("Player"));
    }

    /**
     * ensures the attack behavior does not treat the hero as the player if the player script is removed
     */
    private void OnDestroy()
    {
        transform.parent.gameObject.GetComponent<AttackBehavior>().setLayer(oldLayer);
    }

    /**
     * ensures attacks inputs are being listened to
     */
    private void OnEnable()
    {
        foreach (InputActionReference attack in attacks)
        {
            attack.action.started += onAttackStart;
            attack.action.canceled += onAttackEnd;
            attack.action.Enable();
        }
    }

    /**
     * ensures attacks inputs are being listened to
     */
    private void OnDisable()
    {
        foreach (InputActionReference attack in attacks)
        {
            attack.action.started -= onAttackStart;
            attack.action.canceled -= onAttackEnd;
            attack.action.Disable();
        }
    }

    /**
     * called when the user first clicks an attack button
     */
    private void onAttackStart(InputAction.CallbackContext context)
    {
        player.startAttack(Array.FindIndex(attacks, a => a.action == context.action));
    }

    /**
     * called when the user releases an attack button
     */
    private void onAttackEnd(InputAction.CallbackContext context)
    {
        player.endAttack(Array.FindIndex(attacks, a => a.action == context.action));
    }
}
