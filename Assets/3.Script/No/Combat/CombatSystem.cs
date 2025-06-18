using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;
    
    //private Dictionary<SkillType, SkillEffectHandlerBase> _skillHandlers;

    private void Awake()
    {
        Instance = this;
    
        // _skillHandlers = new Dictionary<SkillType, SkillEffectHandlerBase>
        // {
        //     { SkillType.Damage, new DamageSkillHandler() },
        //     { SkillType.Heal, new HealSkillHandler() },
        //     { SkillType.Buff, new BuffSkillHandler() }
        // };
    }
    
    // public void ExecuteSkill(IDamageAble attackAble, IDamageAble targetAble, int skillIndex)
    // { 
    //     SkillEffectHandlerBase skill = attackAble.Stat.Skills[skillIndex];
    //     RangeSystem.Instance.ResetAllTiles();
    //     RangeSystem.Instance.ShowSkillRange(attackAble, targetAble, skillIndex);
    //     var targets = RangeSystem.Instance.damageAbles;
    //     if (!_skillHandlers.TryGetValue(skill.Type, out var handler))
    //     {
    //         return;
    //     }
    //     
    //     foreach (var target in targets)
    //     {
    //         handler.Apply(attackAble, target, skill);
    //     }
    //     
    //     Turn_Test.Instance.SkillTcs.TrySetResult(true);
    // }

    public void ExecuteSkill(IDamageAble attacker, int skillIndex)
    {
        SkillEffectHandlerBase skill = attacker.Stat.Skills[skillIndex];
        List<IDamageAble> targetList = RangeSystem.Instance.damageAbles;
        
        Debug.Log($"targetList:: {targetList.Count}");
    
        foreach (IDamageAble target in targetList)
        {
            Debug.Log($"ExecuteSkill target :: {target}, skill.Type :: {skill.Type}");
            
            switch (skill.Type)
            {
                case SkillType.Damage:
                    ApplyDamage(attacker, target, attacker.Stat.Attack);
                    break;
    
                case SkillType.Heal:
                    ApplyHeal(attacker, target, attacker.Stat.Attack);
                    break;
                
                case SkillType.Buff:
                    ApplyBuff(attacker, target, attacker.Stat.Attack);
    
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void ApplyDamage(IDamageAble attacker, IDamageAble target, int amount)
    {
        var combatEvent = new CombatEvent
        {
            Sender = attacker,
            Target = target,
            Damage = amount
        };
        
        target.TakeDamage(combatEvent);
    }
    
    public void ApplyHeal(IDamageAble healer, IDamageAble target, int amount)
    {
        var healEvent = new HealEvent
        {
            Sender = healer,
            Target = target,
            Heal = amount,
            Position = target.GameObject.transform.position
        };
        
        target.TakeHeal(healEvent);
    }
    
    public void ApplyBuff(IDamageAble healer, IDamageAble target, int amount)
    {
        var buffEvent = new BuffEvent
        {
            Sender = healer,
            Target = target,
            Buff = amount,
            Position = target.GameObject.transform.position
        };
        
        target.TakeBuff(buffEvent);
    }
}
