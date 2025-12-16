using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void OnDestroy()
    {
        transform.parent.gameObject.GetComponent<AttackBehavior>().setLayer(oldLayer);
    }

    private void OnEnable()
    {
        foreach (InputActionReference attack in attacks)
        {
            attack.action.started += onAttackStart;
            attack.action.canceled += onAttackEnd;
            attack.action.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (InputActionReference attack in attacks)
        {
            attack.action.started -= onAttackStart;
            attack.action.canceled -= onAttackEnd;
            attack.action.Disable();
        }
    }

    private void onAttackStart(InputAction.CallbackContext context)
    {
        player.startAttack(Array.FindIndex(attacks, a => a.action == context.action));
    }

    private void onAttackEnd(InputAction.CallbackContext context)
    {
        player.endAttack(Array.FindIndex(attacks, a => a.action == context.action));
    }
}
