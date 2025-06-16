using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Effect/Heal")]
public class HealSkillHandler : SkillEffectHandlerBase
{
    public override void Apply(IDamageAble attacker, IDamageAble target, SkillEffectHandlerBase skill)
    {
        if (!IsSameTeam(attacker, target)) return;

        Debug.Log("apply heal");
            
        var healEvent = new HealEvent
        {
            Sender = attacker,
            Target = target,
            Heal = attacker.Stat.Attack,
            Position = target.GameObject.transform.position
        };
        target.TakeHeal(healEvent);
    }
}

