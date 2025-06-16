using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;
    
    private Dictionary<SkillType, SkillEffectHandlerBase> _skillHandlers;

    private void Awake()
    {
        Instance = this;

        _skillHandlers = new Dictionary<SkillType, SkillEffectHandlerBase>
        {
            { SkillType.Damage, new DamageSkillHandler() },
            { SkillType.Heal, new HealSkillHandler() },
            { SkillType.Buff, new BuffSkillHandler() }
        };
    }
    
    public void ExecuteSkill(IDamageAble attacker, int skillIndex)
    {
        SkillEffectHandlerBase skill = attacker.Stat.Skills[skillIndex];
        var targets = SkillRangeSystem.Instance.damageAbles;

        if (!_skillHandlers.TryGetValue(skill.Type, out var handler))
        {
            return;
        }

        foreach (var target in targets)
        {
            handler.Apply(attacker, target, skill);
        }
    }

    // public void ExecuteSkill(IDamageAble attacker, int skillIndex)
    // {
    //     SkillSO skill = attacker.Stat.Skills[skillIndex];
    //     List<IDamageAble> targetList = SkillRangeSystem.Instance.damageAbles;
    //
    //     foreach (IDamageAble target in targetList)
    //     {
    //         switch (skill.Type)
    //         {
    //             case SkillType.Damage:
    //                 if (IsSameTeam(attacker, target))
    //                 {
    //                     Debug.Log("같은 팀으로 피격을 넘어갑니다.");
    //                     break;
    //                 }
    //                 
    //                 ApplyDamage(attacker, target, attacker.Stat.Attack);
    //                 break;
    //
    //             case SkillType.Heal:
    //                 ApplyHeal(attacker, target, attacker.Stat.Attack);
    //                 break;
    //             
    //             case SkillType.Buff:
    //                 ApplyBuff(attacker, target, attacker.Stat.Attack);
    //
    //                 break;
    //             
    //             default:
    //                 throw new ArgumentOutOfRangeException();
    //         }
    //     }
    // }

    // private void ApplyDamage(IDamageAble attacker, IDamageAble target, int amount)
    // {
    //     var combatEvent = new CombatEvent
    //     {
    //         Sender = attacker,
    //         Target = target,
    //         Damage = amount
    //     };
    //     
    //     target.TakeDamage(combatEvent);
    // }
    //
    // private void ApplyHeal(IDamageAble healer, IDamageAble target, int amount)
    // {
    //     var healEvent = new HealEvent
    //     {
    //         Sender = healer,
    //         Target = target,
    //         Heal = amount,
    //         Position = target.GameObject.transform.position
    //     };
    //     
    //     target.TakeHeal(healEvent);
    // }
    //
    // private void ApplyBuff(IDamageAble healer, IDamageAble target, int amount)
    // {
    //     var buffEvent = new BuffEvent
    //     {
    //         Sender = healer,
    //         Target = target,
    //         Buff = amount,
    //         Position = target.GameObject.transform.position
    //     };
    //     
    //     target.TakeBuff(buffEvent);
    // }
}
