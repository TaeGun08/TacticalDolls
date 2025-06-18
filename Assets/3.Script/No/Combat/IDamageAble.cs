using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IDamageAble
{
    IStat Stat { get; }
    Collider MainCollider { get; }
    GameObject GameObject { get; }
    int Team { get; }

    Task Excute(int selectedSkill, List<IDamageAble> targets, Transform targetPoint);
    void TakeDamage(CombatEvent combatEvent);
    void TakeHeal(HealEvent combatEvent);
    void TakeBuff(BuffEvent combatEvent);
}