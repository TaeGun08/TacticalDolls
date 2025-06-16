using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public int ID;
    public string WeaponName;
    public int Level;
    public int BaseDamage;
    List<SkillSO> Skills { get; set; }

    public int GetDamage() => BaseDamage + Level;
    
    // public void OnHitEffect(CombatEvent combatEvent, CharacterData owner)
    // {
    //     foreach (SkillSO skill in Skills)
    //     {
    //         switch (skill.Type)
    //         {
    //             case SkillType.Damage:
    //                 Debug.Log($"{WeaponName} Reflect activated! Reflecting {skill.Damage} damage.");
    //                 break;
    //
    //             case SkillType.Heal:
    //                 Debug.Log($"{WeaponName} Bleed activated! Target takes {skill.Damage} bleed damage.");
    //                 break;
    //             
    //             case SkillType.Buff:
    //                 Debug.Log($"{WeaponName} Bleed activated! Target takes {skill.Damage} bleed damage.");
    //                 break;
    //
    //             default:
    //                 Debug.Log($"{WeaponName} activates skill: {skill.Damage}");
    //                 break;
    //         }
    //     }
    // }
}