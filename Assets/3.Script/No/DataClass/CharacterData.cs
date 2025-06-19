using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class CharacterData : MonoBehaviour, IDamageAble 
{
    public int CharacterID;
    public string PrefabName;

    [SerializeField] private StatDataSO baseStatSO;
    [SerializeField] private StatData runtimeStat;
    private IDamageAble damageAbleImplementation;


    public IStat Stat => runtimeStat;

    public Collider MainCollider { get; }
    public GameObject GameObject => gameObject;
    public int Team => 0;
    public SkillParent[] HasSkills { get => hasSkills; set => hasSkills = value; }
    
    public SkillParent[] hasSkills;
    
    private void Awake()
    {
        runtimeStat = new StatData(baseStatSO);
    }

    public async Task Excute(int selectedSkill, List<IDamageAble> targets, Transform targetPoint)
    {
        if (HasSkills[selectedSkill] == null)
        {
            Debug.Log("No Skill Found");
            return;
        }
        await CharacterSequenceManager.Instance.MakeSequence(HasSkills[selectedSkill] as SkillBase, targets, targetPoint);
    }
    

    public void TakeDamage(CombatEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take damage :: CharacterID {CharacterID} _ {combatEvent.Damage}");
        Stat.HP -= combatEvent.Damage;
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }

    public void TakeHeal(HealEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take Heal :: {CharacterID}");
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }

    public void TakeBuff(BuffEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take Buff :: {CharacterID}");
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }

    // 임시 스텟 계산 및 적용
    public StatData CalculateStatFromLevel(int level)
    {
        runtimeStat.Level = level;
        runtimeStat.HP = baseStatSO.hp + level;
        runtimeStat.Attack = baseStatSO.attack + level;
        runtimeStat.Defense = baseStatSO.defense + level;
        runtimeStat.MoveRange = baseStatSO.moveRange + level;
        runtimeStat.Skills = new List<SkillEffectHandlerBase>(baseStatSO.skills);
        runtimeStat.Weapon = baseStatSO.weapon;
        
        return runtimeStat;
    }
    
    public StatData ResetStatFrom()
    {
        runtimeStat.Level = 1;
        runtimeStat.HP = 100;
        runtimeStat.Attack = 100;
        runtimeStat.Defense = 100;
        runtimeStat.MoveRange = 5;
        runtimeStat.Skills = new List<SkillEffectHandlerBase>();
        runtimeStat.Weapon = baseStatSO.weapon;
        
        return runtimeStat;
    }
}

// [System.Serializable]
// public class StatData : IStat
// {
//     [SerializeField] private int level;
//     [SerializeField] private int hp;
//     [SerializeField] private int attack;
//     [SerializeField] private int defense;
//     [SerializeField] private int moveRange;
//     [SerializeField] private List<SkillEffectHandlerBase> skills = new List<SkillEffectHandlerBase>();
//     [SerializeField] private WeaponData weapon;
//
//     public int Level { get => level; set => level = value; }
//     public int HP { get => hp; set => hp = value; }
//     public int Attack { get => attack; set => attack = value; }
//     public int Defense { get => defense; set => defense = value; }
//     public int MoveRange { get => moveRange; set => moveRange = value; }
//     public List<SkillEffectHandlerBase> Skills { get => skills; set => skills = value; }
//     public WeaponData Weapon { get => weapon; set => weapon = value; }
// }


