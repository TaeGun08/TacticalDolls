using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData : MonoBehaviour, IDamageAble
{
    public int CharacterID;
    public string PrefabName;

    [SerializeReference] private StatData stat;
    public WeaponData Weapon;

    public IStat Stat => stat;
    public Collider MainCollider { get; }
    public GameObject GameObject => gameObject;
    public int Team => 0;
    
    public void TakeDamage(CombatEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take damage :: {CharacterID}");
        Stat.HP -= combatEvent.Damage;
    }

    public void TakeHeal(HealEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take Heal :: {CharacterID}");
    }
    
    public void TakeBuff(BuffEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take Buff :: {CharacterID}");
    }
    
    // 임시 스텟 계산 및 적용
    public StatData CalculateStatFromLevel(int level)
    {
        return new StatData
        {
            Level = level ,
            HP = 100 + level,
            Attack = 20 + level + Weapon.GetDamage(),
            Defense = 5 + level,
            MoveRange = 3,
            Skills = new List<SkillSO>(stat.Skills)
        };
    }
}

[System.Serializable]
public class StatData : IStat
{
    [SerializeField] private int level;
    [SerializeField] private int hp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int moveRange;
    [SerializeField] private List<SkillSO> skills = new List<SkillSO>();
    
    public int Level { get => level; set => level = value; }
    public int HP { get => hp; set => hp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int MoveRange { get => moveRange; set => moveRange = value; }
    public List<SkillSO> Skills { get => skills; set => skills = value; }
}


