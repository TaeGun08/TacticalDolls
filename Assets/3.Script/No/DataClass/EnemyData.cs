using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData : MonoBehaviour, IDamageAble
{
    public int EnemyID;
    public string PrefabName;

    [SerializeReference] private EnemyStatData stat;

    public IStat Stat => stat;
    public Collider MainCollider { get; }
    public GameObject GameObject => gameObject;
    public int Team => 1;
    
    public void TakeDamage(CombatEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Enemy Take damage :: {EnemyID}");
        
        Stat.HP -= combatEvent.Damage;
    }

    public void TakeHeal(HealEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Enemy Take Heal :: {EnemyID}");
    }
    
    public void TakeBuff(BuffEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Enemy Take Buff :: {EnemyID}");
    }
}

[System.Serializable]
public class EnemyStatData: IStat
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