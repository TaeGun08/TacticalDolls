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

    public abstract void Apply(IDamageAble attacker, IDamageAble target, SkillEffectHandlerBase skill);
    
    public bool IsSameTeam(IDamageAble a, IDamageAble b)
    {
        return a.Team == b.Team;
    }
}

