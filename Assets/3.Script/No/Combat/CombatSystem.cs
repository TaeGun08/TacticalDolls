using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ExecuteSkill(IDamageAble attacker, int skillIndex)
    {
        CharacterData character = attacker as CharacterData;
        if (character != null)
        {
            // 캐릭터 스킬 처리
        }
        else
        {
            EnemyData enemy = attacker as EnemyData;
            if (enemy != null)
            {
                // 적 스킬 처리
                return;
            }
            else
            {
                Debug.LogWarning("캐스팅 실패: attacker는 CharacterData도 EnemyData도 아님");
            }
        }
        
        List<IDamageAble> targetList = SkillRangeSystem.Instance.damageAbles;
        
        if (character != null)
        {
            // 플레이어 공격 처리
            SkillSO skill = character.Skills[skillIndex];
        
            foreach (IDamageAble target in targetList)
            {
                if (target.Team == attacker.Team)
                {
                    Debug.Log("같은 팀으로 피격을 넘어갑니다.");
                    continue;
                }
                
                switch (skill.Type)
                {
                    case SkillType.Damage:
                        var combatEvent = new CombatEvent
                        {
                            Sender = attacker,
                            Target = target,
                            Damage = character.Stat.Attack,
                        };
                        
                        target.TakeDamage(combatEvent);
                        break;

                    case SkillType.Heal:
                        var healEvent = new HealEvent
                        {
                            Sender = attacker,
                            Target = target,
                            Heal = character.Stat.Attack,
                            Position = target.GameObject.transform.position,
                        };
                        target.TakeHeal(healEvent);
                        break;

                    // 다른 타입도 추가 가능
                }
            }
        }
        else
        {
            // enemy 공격 처리
        }
    }
    
    // 임시 계산
    // private int CalculateDamage(CharacterData caster, IDamageAble target, SkillSO skill)
    // {
    //     int baseDamage = caster.Stat.Attack + caster.Weapon.Damge;
    //     return baseDamage; 
    // }

    // private List<IDamageAble> GetTargetsInRange(Tile centerTile, RangeType rangeType, int range)
    // {
    //     List<IDamageAble> targets = new List<IDamageAble>();
    //     Tile[,] tiles = TileManager.Instance.tiles;
    //
    //     for (int x = 0; x < tiles.GetLength(0); x++)
    //     {
    //         for (int y = 0; y < tiles.GetLength(1); y++)
    //         {
    //             Tile tile = tiles[x, y];
    //             if (tile == null) continue;
    //
    //             int dx = Mathf.Abs(tile.x - centerTile.x);
    //             int dy = Mathf.Abs(tile.y - centerTile.y);
    //
    //             bool inRange = false;
    //
    //             switch (rangeType)
    //             {
    //                 case RangeType.Straight:
    //                 case RangeType.Plus:
    //                     inRange = (dx == 0 && dy <= range) || (dy == 0 && dx <= range);
    //                     break;
    //                 case RangeType.Cross:
    //                     inRange = (dx == dy && dx <= range);
    //                     break;
    //                 case RangeType.Around:
    //                     inRange = (dx + dy) <= range;
    //                     break;
    //             }
    //
    //             if (!inRange) continue;
    //
    //             // 타일 위에 IDamageAble 대상이 있는지 확인
    //             IDamageAble target = tile.GetOccupant() as IDamageAble;
    //             if (target != null)
    //             {
    //                 targets.Add(target);
    //             }
    //         }
    //     }
    //
    //     return targets;
    // }
}
