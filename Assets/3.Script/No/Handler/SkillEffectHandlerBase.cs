using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillEffectHandlerBase : ScriptableObject
{
    public string SkillID;
    public string Name;
    public SkillType Type;
    public RangeType RangeType;
    public int Range;
    public int Damage;

   /// <summary>
   /// 캐릭터가 보유한 기본 스킬 적용
   /// </summary>
    public abstract void Apply(IDamageAble attacker, IDamageAble target, SkillEffectHandlerBase skill);
    
    /// <summary>
    /// 캐릭터가 보유한 무기 스킬 및 특수효과 적용
    /// GetExtraDamage : 추가 데미지 
    /// ApplyAdditionalEffects : 버프, 디버프
    /// </summary>
    public virtual void GetExtraDamage(IDamageAble attacker, IDamageAble target, SkillEffectHandlerBase skill){}
    public virtual void ApplyAdditionalEffects(IDamageAble attacker, IDamageAble target, SkillEffectHandlerBase skill) {}
    
    /// <summary>
    /// 같은 유닛인지 체크
    /// </summary>
    public bool IsSameTeam(IDamageAble a, IDamageAble b)
    {
        return a.Team == b.Team;
    }
}

