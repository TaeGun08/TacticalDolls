using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Effect/Buff")]
public class BuffSkillHandler : SkillEffectHandlerBase
{
    public override void Apply(IDamageAble attacker, IDamageAble target, SkillEffectHandlerBase skill)
    {
        if (!IsSameTeam(attacker, target)) return;
        
        var buffEvent = new BuffEvent
        {
            Sender = attacker,
            Target = target,
            Buff = attacker.Stat.Attack,
            Position = target.GameObject.transform.position
        };
        target.TakeBuff(buffEvent);
    }
}

