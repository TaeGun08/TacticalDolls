using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAble
{
    IStat Stat { get; }
    Collider MainCollider { get; }
    GameObject GameObject { get; }
    int Team { get; }

    void TakeDamage(CombatEvent combatEvent);
    void TakeHeal(HealEvent combatEvent);
    void TakeBuff(BuffEvent combatEvent);
}