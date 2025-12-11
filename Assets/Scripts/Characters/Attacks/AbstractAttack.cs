using UnityEngine;

public abstract class AbstractAttack : ScriptableObject
{
    [SerializeField] protected float damage;
    [SerializeField] protected float staminaCost;
    protected int attackNum;
    protected Animator animator;

    public void init(int attackNum, Animator animator)
    {
        this.attackNum = attackNum;
        this.animator = animator;
    }

    public abstract void startAttack();
    public abstract void applyDamage(AttackBehavior enemy);
    public abstract bool endAttack(bool force = false);
}
