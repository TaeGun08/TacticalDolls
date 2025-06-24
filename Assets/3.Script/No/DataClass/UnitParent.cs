using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UnitParent : MonoBehaviour, IDamageAble
{
    public abstract IStat Stat { get; }
    public abstract Collider MainCollider { get; }
    public abstract GameObject GameObject { get; }
    public abstract int Team { get; }
    public abstract SkillParent[] HasSkills { get; set; }
    public abstract Action OnHpChanged { get; set; }
    
    public abstract Animator Animator { get; }

    public abstract Task Excute(int selectedSkill, List<IDamageAble> targets, Transform targetPoint);
    
    public abstract void TakeDamage(CombatEvent combatEvent);

    public abstract void TakeHeal(HealEvent combatEvent);

    public abstract void TakeBuff(BuffEvent combatEvent);
}
