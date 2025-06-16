using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public int ID;
    public string WeaponName;
    public int Level;
    public int BaseDamage;
    
    public List<SkillEffectHandlerBase> Skills;

    //public int GetDamage() => BaseDamage + Level;
    
    /// <summary>
    /// 다수 이상일 경우 가상함수 사용 또는 내장 함수 사용 편한대로
    /// </summary>
    public void TriggerSkills(IDamageAble attacker, IDamageAble target)
    {
        foreach (SkillEffectHandlerBase skill in Skills)
        {
            switch (skill.Type)
            {
                case SkillType.Damage:
                   
                    skill.GetExtraDamage(attacker, target, skill);
                    skill.ApplyAdditionalEffects(attacker, target, skill);
                    
                    break;
            
                case SkillType.Heal:
                    OnHealEffect(attacker, target, skill);
                    break;
            
                case SkillType.Buff:
                    OnBuffEffect(attacker, target, skill);
                    break;
            
                default:
                    Debug.Log($"{WeaponName} activates skill: {skill.Damage}");
                    break;
            }
        }
    }
    
    public void OnDamageEffect(IDamageAble attacker, IDamageAble target,  SkillEffectHandlerBase skill)
    {
        if (skill.IsSameTeam(attacker, target))
        {
            Debug.Log("같은 편 공격력 증가");
            target.Stat.Attack += 10;

            return;
        }
        
        Debug.Log("적 추가 hp 감소");
        target.Stat.HP -= 10;
    }
    
    public void OnHealEffect(IDamageAble attacker, IDamageAble target,  SkillEffectHandlerBase skill)
    {
        if (skill.IsSameTeam(attacker, target))
        {
            Debug.Log("같은 편 HP 증가");
            attacker.Stat.HP += 100;

            return;
        }
        
        Debug.Log("적 추가 hp 감소");
        target.Stat.HP -= 10;
    }
    
    public void OnBuffEffect(IDamageAble attacker, IDamageAble target, SkillEffectHandlerBase skill)
    {
        if (skill.IsSameTeam(attacker, target))
        {
            Debug.Log("같은 편 이동범위 증가");
            attacker.Stat.MoveRange += 5;

            return;
        }
        
        Debug.Log("적 이동범위 감소");
        target.Stat.MoveRange -= 1;
    }
}