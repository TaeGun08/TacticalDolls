using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatData : IStat
{
    [SerializeField] private int level;
    [SerializeField] private int hp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int moveRange;
    [SerializeField] private List<SkillEffectHandlerBase> skills = new();
    [SerializeField] private WeaponData weapon;

    public int Level { get => level; set => level = value; }
    public int HP { get => hp; set => hp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int MoveRange { get => moveRange; set => moveRange = value; }
    public List<SkillEffectHandlerBase> Skills { get => skills; set => skills = value; }
    public WeaponData Weapon { get => weapon; set => weapon = value; }

    public StatData() {}

    public StatData(StatDataSO baseSO)
    {
        Level = baseSO.level;
        HP = baseSO.hp;
        Attack = baseSO.attack;
        Defense = baseSO.defense;
        MoveRange = baseSO.moveRange;
        Skills = new List<SkillEffectHandlerBase>(baseSO.skills);
        Weapon = baseSO.weapon;
    }
}
