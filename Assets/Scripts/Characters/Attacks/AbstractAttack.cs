using UnityEngine;


/**
 * a base class for hero attacks that handles animations and damage application
 */
public abstract class AbstractAttack : ScriptableObject
{
    [SerializeField] protected float damage;
    [SerializeField] protected float staminaCost;
    protected int attackNum;
    protected Animator animator;

    public float Damage => damage;
    public float StaminaCost => staminaCost;

    public AbstractAttack init(int attackNum, Animator animator)
    {
        AbstractAttack attack = Instantiate(this);
        attack.attackNum = attackNum;
        attack.animator = animator;
        return attack;
    }

    /**
     * will be called by AttackBehavior to begin the attack
     */
    public abstract void startAttack();

    /**
     * called by attack behavior to apply damage to an enemy hitbox crosses with the hero hurtbox
     */
    public abstract void applyDamage(AttackBehavior enemy);

    /**
     * called by AttackBehavior if the user ends the attack or force ends it when the animation ends
     * 
     * @param force whether the attack is being force ended
     * @return whether the attack has successfully ended
     */
    public abstract bool endAttack(bool force = false);
}
