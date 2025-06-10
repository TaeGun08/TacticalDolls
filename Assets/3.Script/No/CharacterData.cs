using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData : MonoBehaviour
{
    public int CharacterID;        
    public string PrefabName;       
    public StatData Stat;
    public WeaponData Weapon;
    public List<SkillSO> Skills;
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
    public int HP;
    public int Attack;
    public int Defense;
    public int Speed;
}


