using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private InputActionReference[] attacks;

    private AttackBehavior player;
    private static PlayerAttackController singleton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (singleton is not null)
            throw new InvalidOperationException("Only one Player Attack Controller should exist at a time.");
        singleton = this;
        player = GetComponent<AttackBehavior>();
        gameObject.layer = LayerMask.NameToLayer("Player");
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
            attack.action.started += onAttackStart;
            attack.action.canceled += onAttackEnd;
            attack.action.Enable();
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
