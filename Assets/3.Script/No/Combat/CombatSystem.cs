using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ExecuteSkill(IDamageAble attacker, int skillIndex)
    {
        switch (attacker)
        {
            case CharacterData character:
                ExecuteCharacterSkill(character, skillIndex);
                break;
            case EnemyData enemy:
                ExecuteEnemySkill(enemy, skillIndex);
                break;
            default:
                Debug.LogWarning("캐스팅 실패");
                break;
        }
    }

    private void ExecuteCharacterSkill(CharacterData character, int skillIndex)
    {
        SkillSO skill = character.Skills[skillIndex];
        List<IDamageAble> targetList = SkillRangeSystem.Instance.damageAbles;

        foreach (IDamageAble target in targetList)
        {
            switch (skill.Type)
            {
                case SkillType.Damage:
                    if (IsSameTeam(character, target))
                    {
                        Debug.Log("같은 팀으로 피격을 넘어갑니다.");
                        return;
                    }
                    
                    ApplyDamage(character, target, character.Stat.Attack);
                    break;

                case SkillType.Heal:
                    ApplyHeal(character, target, character.Stat.Attack);
                    break;
                
                case SkillType.Buff:
                    ApplyBuff(character, target, character.Stat.Attack);

                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ExecuteEnemySkill(EnemyData enemy, int skillIndex)
    {
        SkillSO skill = enemy.Skills[skillIndex];
        List<IDamageAble> targetList = SkillRangeSystem.Instance.damageAbles;

        foreach (IDamageAble target in targetList)
        {
            switch (skill.Type)
            {
                case SkillType.Damage:
                    if (IsSameTeam(enemy, target))
                    {
                        Debug.Log("같은 팀으로 Enemy로 피격을 넘어갑니다.");
                        return;
                    }
                    
                    ApplyDamage(enemy, target, enemy.Stat.Attack);
                    break;

                case SkillType.Heal:
                    ApplyHeal(enemy, target, enemy.Stat.Attack);
                    break;
                
                case SkillType.Buff:
                    ApplyBuff(enemy, target, enemy.Stat.Attack);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private bool IsSameTeam(IDamageAble a, IDamageAble b)
    {
        return a.Team == b.Team;
    }

    private void ApplyDamage(IDamageAble attacker, IDamageAble target, int amount)
    {
        var combatEvent = new CombatEvent
        {
            Sender = attacker,
            Target = target,
            Damage = amount
        };
        
        target.TakeDamage(combatEvent);
    }

    private void ApplyHeal(IDamageAble healer, IDamageAble target, int amount)
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
    
    private void ApplyBuff(IDamageAble healer, IDamageAble target, int amount)
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
