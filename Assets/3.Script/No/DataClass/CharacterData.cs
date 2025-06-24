using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class CharacterData : UnitParent
{
    private static readonly int ATTACKED = Animator.StringToHash("Attacked");
    public int CharacterID;
    public string PrefabName;

    [SerializeField] private StatDataSO baseStatSO;
    [SerializeField] private StatData runtimeStat;
    private IDamageAble damageAbleImplementation;


    public override IStat Stat => runtimeStat;

    public override Collider MainCollider { get; }
    public override GameObject GameObject => gameObject;
    public override int Team => 0;
    public override SkillParent[] HasSkills { get => hasSkills; set => hasSkills = value; }
    public override Action OnHpChanged { get; set; }
    
    [SerializeField] private Animator animator;
    public override Animator Animator => animator;

    public SkillParent[] hasSkills;
    
    public Sprite characterIcon;

    //InGame
    // private List<BuffParent> Buffs = new List<BuffParent>();
    
    private void Awake()
    {
        runtimeStat = new StatData(baseStatSO);
        runtimeStat.MaxHP = runtimeStat.HP;
    }

    // private void OnEnable()
    // {
    //     if(TurnManager.Instance != null)
    //         TurnManager.Instance.TurnEnded += BuffTurnMinus;
    // }
    //
    // private void OnDisable()
    // {
    //     if(TurnManager.Instance != null)
    //         TurnManager.Instance.TurnEnded -= BuffTurnMinus;
    // }
    //
    // public void BuffTurnMinus() //버프 턴 감소, 0이 될 시 제거
    // {
    //     for (int i = 0; i < Buffs.Count; i++)
    //     {
    //         Buffs[i].remainingTurns -= 1;
    //         
    //         if (Buffs[i].remainingTurns <= 0)
    //         {
    //             Buffs.RemoveAt(i);
    //         }
    //     }
    // }

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
        Debug.Log($"{PrefabName} Character Take damage :: CharacterID {CharacterID} _ {combatEvent.Damage}");
        Stat.HP -= combatEvent.Damage;
        
        animator.SetTrigger(ATTACKED);
        OnHpChanged?.Invoke();
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }

    public override void TakeHeal(HealEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take Heal :: {CharacterID}");
        Stat.HP += combatEvent.Heal;
        
        OnHpChanged?.Invoke();
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }

    public override void TakeBuff(BuffEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take Buff :: {CharacterID}");
        //combatEvent.Sender.Stat.Weapon.TriggerSkills(combatEvent.Sender, this);
    }

    // 임시 스텟 계산 및 적용
    public StatData CalculateStatFromLevel(int characterLevel, int weaponLevel, WeaponData weapon)
    {
        runtimeStat.Level = characterLevel;

        // 스탯 증가 공식
        runtimeStat.HP = baseStatSO.hp + characterLevel * 10;
        runtimeStat.MaxHP = runtimeStat.HP;
        runtimeStat.Attack = baseStatSO.attack + characterLevel * 2 + weaponLevel * 5;
        runtimeStat.Defense = baseStatSO.defense + characterLevel * 2;
        runtimeStat.MoveRange = baseStatSO.moveRange + characterLevel / 5; // 5레벨마다 1 증가

        // 스킬 및 무기 정보 복사
        runtimeStat.Weapon = weapon;

        return runtimeStat;
    }
    
    public StatData ResetStatFrom()
    {
        runtimeStat.Level = 1;
        runtimeStat.HP = 100;
        runtimeStat.Attack = 100;
        runtimeStat.Defense = 100;
        runtimeStat.MoveRange = 5;
        runtimeStat.Weapon = baseStatSO.weapon;
        
        return runtimeStat;
    }
    
    // 캐릭터 레벨업 요청
    public async Task<bool> UpdateCharacterLevel(int characterCode, int levelPoint)
    {
        string userId = FirebaseMainSession.Instance.FirebaseUser.UserData.UserId;
    
        Debug.Log($"Character Level Up Request ::  {userId}");

        // 1. 데이터 로드
        PlayerDataSample playerData = await FirestoreManager.Instance.ReadDataAsync<PlayerDataSample>(
            FirebaseCollections.Players,
            userId
        );

        if (playerData == null)
        {
            Debug.LogWarning("플레이어 데이터를 찾을 수 없습니다.");
            return false;
        }

        // 2. 캐릭터 찾기
        CharacterDataSample characterToUpdate = playerData.HasCharacter.Find(c => c.characterCode == characterCode);

        if (characterToUpdate == null)
        {
            Debug.LogWarning($"characterCode {characterCode} 에 해당하는 캐릭터를 찾을 수 없습니다.");
            return false;
        }

        // 3. 캐릭터 레벨 업데이트
        characterToUpdate.level += levelPoint;

        // 4. 전체 캐릭터 리스트를 업데이트 필드로 설정
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "HasCharacter", playerData.HasCharacter }
        };

        // 5. Firestore에 반영
        await FirestoreManager.Instance.UpdateDataAsync(FirebaseCollections.Players, userId, updates);
    
        Debug.Log($"캐릭터 {characterCode} 레벨이 {levelPoint}만큼 증가했습니다.");
        
        return true;
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


