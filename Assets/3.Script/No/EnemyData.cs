using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData : MonoBehaviour
{
    public int EnemyID;        
    public string PrefabName;       
    public EnemyStatData Stat;
}

[System.Serializable]
public class EnemyStatData
{
    public int HP;
    public int Attack;
    public int Defense;
    public int MoveRange;
}