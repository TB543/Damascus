using System.Collections.Generic;
using UnityEngine;


/**
 * a class for the most basic attack type, which directly damages enemies in its hitbox
 * enemies can only be damaged once per attack instance
 * attack only ends when animation ends
 * 
 * see AbstractAttack for more information
 */
[CreateAssetMenu(fileName = "BasicAttack", menuName = "Scriptable Objects/BasicAttack")]
public class BasicAttack : AbstractAttack
{
    private bool running = false;
    private HashSet<AttackBehavior> enemiesHit = new();

    public override void startAttack()
    {
        animator.SetTrigger("Attack" + attackNum);
        running = true;
    }

    public override void applyDamage(AttackBehavior enemy)
    {
        if (!enemiesHit.Contains(enemy))
        {
            enemiesHit.Add(enemy);
            enemy.takeDamage(damage);
        }
    }

    public override bool endAttack(bool force = false)
    {
        if (force)
        {
            enemiesHit.Clear();
            running = false;
        }
        return !running;
    }
}
