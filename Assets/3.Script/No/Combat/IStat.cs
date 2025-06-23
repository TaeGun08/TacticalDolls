using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStat
{
    int Level { get; set; }
    int HP { get; set; }
    int MaxHP { get; set; }
    int Attack { get; set; }
    int Defense { get; set; }
    // int Barrier { get; set; }
    int MoveRange { get; set; }
    List<SkillEffectHandlerBase> Skills { get; set; }
    WeaponData Weapon { get; set; }
    
    bool IsDead { get; set;  }
    bool IsCompleteAction { get; set; }
}
