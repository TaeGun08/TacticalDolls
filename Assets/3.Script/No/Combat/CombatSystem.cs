using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ExecuteSkill(CharacterData character, int skillIndex)
    {
        SkillSO skill = character.Skills[skillIndex];
        Tile centerTile = TileManager.Instance.GetClosestTile(character.GameObject.transform.position);

        if (centerTile == null) return;

        List<IDamageAble> targets = GetTargetsInRange(centerTile, skill.RangeType, skill.Range);
        
        foreach (IDamageAble target in targets)
        {
            // 이벤트 객체 생성
            CombatEvent combatEvent = new CombatEvent
            {
                Attacker = character,
                Target = target,
            };

            // 데미지 적용
            target.TakeDamage(combatEvent);
        }
    }
    
    // 임시 계산
    // private int CalculateDamage(CharacterData caster, IDamageAble target, SkillSO skill)
    // {
    //     int baseDamage = caster.Stat.Attack + caster.Weapon.Damge;
    //     return baseDamage; 
    // }

    private List<IDamageAble> GetTargetsInRange(Tile centerTile, RangeType rangeType, int range)
    {
        List<IDamageAble> targets = new List<IDamageAble>();
        Tile[,] tiles = TileManager.Instance.tiles;

        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Tile tile = tiles[x, y];
                if (tile == null) continue;

                int dx = Mathf.Abs(tile.x - centerTile.x);
                int dy = Mathf.Abs(tile.y - centerTile.y);

                bool inRange = false;

                switch (rangeType)
                {
                    case RangeType.Straight:
                    case RangeType.Plus:
                        inRange = (dx == 0 && dy <= range) || (dy == 0 && dx <= range);
                        break;
                    case RangeType.Cross:
                        inRange = (dx == dy && dx <= range);
                        break;
                    case RangeType.Around:
                        inRange = (dx + dy) <= range;
                        break;
                }

                if (!inRange) continue;

                // 타일 위에 IDamageAble 대상이 있는지 확인
                IDamageAble target = tile.GetOccupant() as IDamageAble;
                if (target != null)
                {
                    targets.Add(target);
                }
            }
        }

        return targets;
    }
}
