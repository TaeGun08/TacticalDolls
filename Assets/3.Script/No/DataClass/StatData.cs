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
    [SerializeField] private int attackRange;
    [SerializeField] private WeaponData weapon;
    [SerializeField] private bool isDead;
    [SerializeField] private bool isCompleteAction;
    
    public int Level { get => level; set => level = value; }
    public int HP { get => hp; set => hp = value; }
    public int MaxHP { get; set; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    // public int Barrier { get; set; }
    public int MoveRange { get => moveRange; set => moveRange = value; }
    public int AttackRnage { get => attackRange; set => attackRange = value; }
    public WeaponData Weapon { get => weapon; set => weapon = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsCompleteAction { get => isCompleteAction; set => isCompleteAction = value; }
    public bool isCovered { get; set; }

    public StatData() {}

    public StatData(StatDataSO baseSO)
    {
        Level = baseSO.level;
        HP = baseSO.hp;
        Attack = baseSO.attack;
        Defense = baseSO.defense;
        MoveRange = baseSO.moveRange;
        Weapon = baseSO.weapon;
        IsDead = baseSO.isDead;
        IsCompleteAction = baseSO.isCompleteAction;
    }
}
