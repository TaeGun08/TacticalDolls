using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/StatData")]
public class StatDataSO : ScriptableObject, IStat
{
    public int level;
    public int hp;
    public int attack;
    public int defense;
    public int moveRange;
    public List<SkillEffectHandlerBase> skills = new();
    public WeaponData weapon;
    public bool isDead;
    public bool isCompleteAction;
    private IStat statImplementation;

    public int Level { get => level; set => level = value; }
    public int HP { get => hp; set => hp = value; }
    public int MaxHP { get; set; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int MoveRange { get => moveRange; set => moveRange = value; }
    public List<SkillEffectHandlerBase> Skills { get => skills; set => skills = value; }
    public WeaponData Weapon { get => weapon; set => weapon = value; }

    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsCompleteAction { get => isCompleteAction; set => isCompleteAction = value; }
}
