using System.Collections.Generic;
using UnityEngine;

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
