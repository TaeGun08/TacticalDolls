using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Effect/Damage")]
public class DamageSkillHandler : SkillEffectHandlerBase
{
    public override void Apply(IDamageAble attacker, IDamageAble target, SkillEffectHandlerBase skill)
    {
        if (IsSameTeam(attacker, target)) return;

        var combatEvent = new CombatEvent
        {
            Sender = attacker,
            Target = target,
            Damage = attacker.Stat.Attack
        };
        target.TakeDamage(combatEvent);
    }
}
