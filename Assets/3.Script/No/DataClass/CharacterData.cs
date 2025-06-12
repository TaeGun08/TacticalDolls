using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData : MonoBehaviour, IDamageAble
{
    public int CharacterID;
    public string PrefabName;       
    public StatData Stat;
    public WeaponData Weapon;
    public List<SkillSO> Skills;
    
    public Collider MainCollider { get; }
    public GameObject GameObject => gameObject;
    public int Team => 0; 
    
    public void TakeDamage(CombatEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take damage :: {CharacterID}");
    }

    public void TakeHeal(HealEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Character Take Heal :: {CharacterID}");
    }
    
    // 임시 스텟 계산
    public StatData CalculateStatFromLevel(int level)
    {
        return new StatData
        {
            Level = level,
            HP = 100 + level,
            Attack = 20 + level,
            Defense = 5 + level,
            MoveRange = 3
        };
    }
    
    // 무기 설정
    public void SetWeaponData(int level){}

    // public void test()
    // {
    //     CombatSystem.Instance.ExecuteSkill(this, selectedSkillIndex);
    // }
}

[System.Serializable]
public class WeaponData
{
    public string Name;
    public int Damge;
    public int Speed;
}

[System.Serializable]
public class StatData
{
    public int Level;
    public int HP;
    public int Attack;
    public int Defense;
    public int MoveRange;
}


