using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class EnemyData : UnitParent
{
    public int EnemyID;
    public override string PrefabName { get => prefabName; set => prefabName = value; }
    public string prefabName;

    [SerializeField] private StatDataSO baseStatSO;
    [SerializeField] private StatData runtimeStat;
    
    public override SkillParent[] HasSkills { get => hasSkills; set => hasSkills = value; }
    public override Action OnHpChanged { get; set; }

    public SkillParent[] hasSkills;
    
    public override IStat Stat => runtimeStat;
    
    public override Collider MainCollider { get; }
    public override GameObject GameObject => gameObject;
    
    [SerializeField] private Animator animator;
    public override Animator Animator => animator;
    
    public override int Team => 1;
    
    private void Awake()
    {
        runtimeStat = new StatData(baseStatSO);
    }

    private void Start()
    {
        SetStateSO();
    }

    public void SetStateSO()
    {
        runtimeStat.HP = baseStatSO.hp;
        runtimeStat.MaxHP = runtimeStat.HP;
        runtimeStat.Attack = baseStatSO.attack;
        runtimeStat.Defense = baseStatSO.defense;
        runtimeStat.MoveRange = baseStatSO.moveRange;
        runtimeStat.AttackRnage = baseStatSO.AttackRnage;
    }

    public override async Task Excute(int selectedSkill, List<IDamageAble> targets, Transform targetPoint)
    {
        if (HasSkills[selectedSkill] == null)
        {
            Debug.Log("No Skill Found");
            return;
        }
        await CharacterSequenceManager.Instance.MakeSequence(HasSkills[selectedSkill] as SkillBase, targets, targetPoint);
    }

    
    
    public override void TakeDamage(CombatEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Enemy Take damage :: {EnemyID}");
        Stat.HP -= combatEvent.Damage;
        
        if (Stat.HP <= 0)
        {
            StartCoroutine(DeadCorotuine());
        }
        else
        {
            OnHpChanged?.Invoke();
            animator.SetTrigger(ATTACKED);
        }
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
        
    }

    public override void TakeHeal(HealEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Enemy Take Heal :: {EnemyID}");
        Stat.HP += combatEvent.Heal;
        
        OnHpChanged?.Invoke();
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }
    
    public override void TakeBuff(BuffEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Enemy Take Buff :: {EnemyID}");
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }
}

// [System.Serializable]
// public class EnemyStatData: IStat
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