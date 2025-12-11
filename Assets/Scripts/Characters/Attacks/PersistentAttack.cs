using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PersistentAttack", menuName = "Scriptable Objects/PersistentAttack")]
public class PersistentAttack : AbstractAttack
{
    [SerializeField] private float interval;
    private Dictionary<AttackBehavior, float> lastHitTime = new();

    public override void startAttack()
    {
        animator.SetBool("Attack1", true);
    }

    public override void applyDamage(AttackBehavior enemy)
    {
        if (!lastHitTime.ContainsKey(enemy))
            lastHitTime[enemy] = Time.time - interval;
        if (Time.time - lastHitTime[enemy] >= interval)
        {
            enemy.takeDamage(damage);
            lastHitTime[enemy] = Time.time;
        }
    }

    public override bool endAttack(bool force = false)
    {
        foreach (KeyValuePair<AttackBehavior, float> enemy in lastHitTime.Where(p => Time.time - p.Value > interval).ToArray())
            lastHitTime.Remove(enemy.Key);
        animator.SetBool("Attack1", false);
        return true;
    }
}
