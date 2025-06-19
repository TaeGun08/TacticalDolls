using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class EnemyData : MonoBehaviour, IDamageAble
{
    public int EnemyID;
    public string PrefabName;

    [SerializeField] private StatDataSO baseStatSO;
    [SerializeField] private StatData runtimeStat;
    
    public SkillParent[] HasSkills { get => hasSkills; set => hasSkills = value; }

    public SkillParent[] hasSkills;
    
    public IStat Stat => runtimeStat;
    
    public Collider MainCollider { get; }
    public GameObject GameObject => gameObject;
    public int Team => 1;
    
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
        Debug.Log($"{PrefabName} Enemy Take damage :: {EnemyID}");
        Stat.HP -= combatEvent.Damage;
        
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }

    public void TakeHeal(HealEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Enemy Take Heal :: {EnemyID}");
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }
    
    public void TakeBuff(BuffEvent combatEvent)
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