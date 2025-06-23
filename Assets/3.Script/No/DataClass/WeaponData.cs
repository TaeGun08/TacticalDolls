using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum WeaponType
{
    Rifle,
    Pistol,
    Sword
}

public enum WeaponGrade
{
    Normal,
    Rare,
    Epic,
    Unique
}

public class WeaponData : MonoBehaviour
{
    private const int NO_CHARACTER = -1; 
    
    public int ID;
    public string WeaponName;
    public int Level;
    public int Damage;
    //public int CurrentCharacter;

    public WeaponType WeaponType;
    public WeaponGrade WeaponGrade;

    public Sprite WeaponIcon;
    
    public async Task<bool> UpdateWeaponLevel(int weaponCode, int levelPoint)
    {
        string userId = FirebaseMainSession.Instance.FirebaseUser.UserData.UserId;
        
        Debug.Log($"Weapon Level Up Request ::  {userId}");
        
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

        // 2. 무기 찾기
        WeaponDataSample weaponToUpdate = playerData.HasWeapon.Find(w => w.weaponCode == weaponCode);
    
        if (weaponToUpdate == null)
        {
            Debug.LogWarning($"weaponCode {weaponCode} 에 해당하는 무기를 찾을 수 없습니다.");
            return false;
        }

        // 3. 무기 레벨 업데이트
        weaponToUpdate.level = weaponToUpdate.level + levelPoint;

        // 4. 전체 무기 리스트를 업데이트 필드로 설정
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "HasWeapon", playerData.HasWeapon },
            { "HasCharacter", playerData.HasCharacter }
        };
        
        // 5. 무기가 적용된 캐릭터 스텟도 업데이트
        foreach (var c in playerData.HasCharacter)
        {
            if (c.weapon.weaponCode == weaponCode)
            {
                c.weapon.weaponCode = weaponToUpdate.weaponCode;
                c.weapon.level = weaponToUpdate.level;
            }
        }
        
        // 6. Firestore에 반영
        await FirestoreManager.Instance.UpdateDataAsync(FirebaseCollections.Players, userId, updates);
        
        return true;
    }
    
    // 장착중인 캐릭터가 있는지 체크 추가 필요
    public async void UpdateCharacterCurrentWeapon(int characterCode, int weaponCodeToEquip)
    {
        string userId = FirebaseMainSession.Instance.FirebaseUser.UserData.UserId;

        // 1. 유저 데이터 로드
        PlayerDataSample playerData = await FirestoreManager.Instance.ReadDataAsync<PlayerDataSample>(
            FirebaseCollections.Players,
            userId
        );

        if (playerData == null)
        {
            Debug.LogWarning("플레이어 데이터를 찾을 수 없습니다.");
            return;
        }

        // 2. 장착 대상 캐릭터 및 장착 무기
        CharacterDataSample character = playerData.HasCharacter.Find(c => c.characterCode == characterCode);
        WeaponDataSample newWeapon = playerData.HasWeapon.Find(w => w.weaponCode == weaponCodeToEquip);

        if (character == null || newWeapon == null)
        {
            Debug.LogWarning("캐릭터 또는 무기를 찾을 수 없습니다.");
            return;
        }

        // 3. 이미 다른 캐릭터가 이 무기를 장착 중인지 확인
        if (newWeapon.currentCharacter != characterCode)
        {
            Debug.LogWarning($"이 무기는 캐릭터 {newWeapon.currentCharacter}가 사용 중입니다.");
            return;
        }

        // 4. 기존에 무기 끼고 있던 캐릭터 해제
        foreach (var c in playerData.HasCharacter)
        {
            if (c.weapon.weaponCode == weaponCodeToEquip && c.characterCode != characterCode)
            {
                c.weapon = null;
            }
        }

        // 5. 기존 무기의 currentCharacter 해제
        foreach (var w in playerData.HasWeapon)
        {
            if (w.currentCharacter == characterCode)
                w.currentCharacter = NO_CHARACTER;
        }

        // 6. 무기 장착
        character.weapon = newWeapon;
        newWeapon.currentCharacter = characterCode;

        // 7. Firestore에 업데이트
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "HasCharacter", playerData.HasCharacter },
            { "HasWeapon", playerData.HasWeapon }
        };

        await FirestoreManager.Instance.UpdateDataAsync(FirebaseCollections.Players, userId, updates);

        Debug.Log($"캐릭터 {characterCode}에 무기 {weaponCodeToEquip} 장착 완료");

        // 스탯 업데이트
        PlayerManager.Instance.UpdateCharacterData();
    }
    
    //public List<SkillEffectHandlerBase> Skills;
    //public int GetDamage() => BaseDamage + Level;
    
    /// <summary>
    /// 다수 이상일 경우 가상함수 사용 또는 내장 함수 사용 편한대로
    /// </summary>
    // public void TriggerSkills(IDamageAble attacker, IDamageAble target)
    // {
    //     foreach (SkillEffectHandlerBase skill in Skills)
    //     {
    //         switch (skill.Type)
    //         {
    //             case SkillType.Damage:
    //                 OnDamageEffect(attacker, target, skill);
    //                 // skill.GetExtraDamage(attacker, target, skill);
    //                 // skill.ApplyAdditionalEffects(attacker, target, skill);
    //                 
    //                 break;
    //         
    //             case SkillType.Heal:
    //                 OnHealEffect(attacker, target, skill);
    //                 break;
    //         
    //             case SkillType.Buff:
    //                 OnBuffEffect(attacker, target, skill);
    //                 break;
    //         
    //             default:
    //                 Debug.Log($"{WeaponName} activates skill: {skill.Damage}");
    //                 break;
    //         }
    //     }
    // }
    
    // public void OnDamageEffect(IDamageAble attacker, IDamageAble target,  SkillEffectHandlerBase skill)
    // {
    //     if (skill.IsSameTeam(attacker, target))
    //     {
    //         Debug.Log("같은 편 공격력 증가");
    //         target.Stat.Attack += 10;
    //
    //         return;
    //     }
    //     
    //     Debug.Log("적 추가 hp 감소");
    //     target.Stat.HP -= 10;
    // }
    //
    // public void OnHealEffect(IDamageAble attacker, IDamageAble target,  SkillEffectHandlerBase skill)
    // {
    //     if (skill.IsSameTeam(attacker, target))
    //     {
    //         Debug.Log("같은 편 HP 증가");
    //         attacker.Stat.HP += 100;
    //
    //         return;
    //     }
    //     
    //     Debug.Log("적 추가 hp 감소");
    //     target.Stat.HP -= 10;
    // }
    //
    // public void OnBuffEffect(IDamageAble attacker, IDamageAble target, SkillEffectHandlerBase skill)
    // {
    //     if (skill.IsSameTeam(attacker, target))
    //     {
    //         Debug.Log("같은 편 이동범위 증가");
    //         attacker.Stat.MoveRange += 5;
    //
    //         return;
    //     }
    //     
    //     Debug.Log("적 이동범위 감소");
    //     target.Stat.MoveRange -= 1;
    // }
}