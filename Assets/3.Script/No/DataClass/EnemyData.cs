using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData : MonoBehaviour, IDamageAble
{
    public int EnemyID;        
    public string PrefabName;       
    public EnemyStatData Stat;
    
    public Collider MainCollider { get; }
    public GameObject GameObject => gameObject;
    
    public void TakeDamage(CombatEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Enemy Take damage :: {EnemyID}");
    }

    public void TakeHeal(HealEvent combatEvent)
    {
        Debug.Log($"{PrefabName} Enemy Take Heal :: {EnemyID}");
    }
}

[System.Serializable]
public class EnemyStatData
{
    public int HP;
    public int Attack;
    public int Defense;
    public int MoveRange;
}