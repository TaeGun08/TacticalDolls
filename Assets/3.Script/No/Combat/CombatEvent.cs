using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InGameEvent
{
    public enum EventType
    {
        Unknown,
        Combat,
        Heal,
    }
    
    public IDamageAble Attacker { get; set; }
    public IDamageAble Target { get; set; }
    public abstract EventType Type { get; }
}

public class CombatEvent : InGameEvent
{
    public int Damage;
    public override EventType Type => EventType.Combat;
}

public class HealEvent : InGameEvent
{
    public int Heal { get; set; }
    public Vector3 Position { get; set; }
    public override EventType Type => EventType.Heal;
}