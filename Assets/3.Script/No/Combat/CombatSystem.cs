using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;
    
    //private Dictionary<SkillType, SkillEffectHandlerBase> _skillHandlers;

    private void Awake()
    {
        Instance = this;
    
        // _skillHandlers = new Dictionary<SkillType, SkillEffectHandlerBase>
        // {
        //     { SkillType.Damage, new DamageSkillHandler() },
        //     { SkillType.Heal, new HealSkillHandler() },
        //     { SkillType.Buff, new BuffSkillHandler() }
        // };
    }
    
    // public void ExecuteSkill(IDamageAble attackAble, IDamageAble targetAble, int skillIndex)
    // { 
    //     SkillEffectHandlerBase skill = attackAble.Stat.Skills[skillIndex];
    //     RangeSystem.Instance.ResetAllTiles();
    //     RangeSystem.Instance.ShowSkillRange(attackAble, targetAble, skillIndex);
    //     var targets = RangeSystem.Instance.damageAbles;
    //     if (!_skillHandlers.TryGetValue(skill.Type, out var handler))
    //     {
    //         return;
    //     }
    //     
    //     foreach (var target in targets)
    //     {
    //         handler.Apply(attackAble, target, skill);
    //     }
    //     
    //     Turn_Test.Instance.SkillTcs.TrySetResult(true);
    // }

    // public void ExecuteSkill(IDamageAble attacker, int skillIndex)
    // {
    //     SkillEffectHandlerBase skill = attacker.Stat.Skills[skillIndex];
    //     List<IDamageAble> targetList = RangeSystem.Instance.damageAbles;
    //     
    //     Debug.Log($"targetList:: {targetList.Count}");
    //
    //     foreach (IDamageAble target in targetList)
    //     {
    //         Debug.Log($"ExecuteSkill target :: {target}, skill.Type :: {skill.Type}");
    //         
    //         switch (skill.Type)
    //         {
    //             case SkillType.Damage:
    //                 ApplyDamage(attacker, target, attacker.Stat.Attack);
    //                 break;
    //
    //             case SkillType.Heal:
    //                 ApplyHeal(attacker, target, attacker.Stat.Attack);
    //                 break;
    //             
    //             case SkillType.Buff:
    //                 ApplyBuff(attacker, target, attacker.Stat.Attack);
    //
    //                 break;
    //             
    //             default:
    //                 throw new ArgumentOutOfRangeException();
    //         }
    //     }
    // }

    /// <summary>
    /// 엄폐 방향에 따라 감소된 대미지의 값을 받아오는 함수
    /// </summary>
    /// <param name="attackerPos"></param>
    /// <param name="targetPos"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    private float GetRelativeAttackAngle(Vector2 attackerPos, Vector2 targetPos, Tile tile)
    {
        Vector2 attackDir = (attackerPos - targetPos).normalized;

        Vector2 fowardDir = Vector2.zero;
        
        //엄폐 방향
        switch (tile.obstacleDir)
        {
            case 1: //정면
                fowardDir = Vector2.up;
                Debug.Log("정면");
                break;
            case 2: //후면
                fowardDir = Vector2.down;
                Debug.Log("후면");
                break;
            case 3: //좌
                fowardDir = Vector2.left;
                Debug.Log("측면 - 좌");
                break;
            case 4: //우
                fowardDir = Vector2.right;
                Debug.Log("측면 - 우");
                break;
        }
        
        // 기준 벡터: forward
        // 공격 방향: attackDir
        float angle = Vector2.SignedAngle(fowardDir, attackDir);
        Debug.Log($"방향 : {angle}");
        
        float damage = 1f;
        if (angle < 90 && angle > -90 && tile.obstacleDir != 0)
        {
            damage = 0.5f;
            Debug.Log("뎀지 감소");
        }
        
        // 결과는 -180도 ~ +180도 사이
        return damage;
    }
    
    public void ApplyDamage(IDamageAble attacker, List<IDamageAble> targets, int amount)
    {
        //공격자의 현재 타일
        
        //공격자 위치
        Vector2 attackerPos = new Vector2(attacker.GameObject.transform.position.x,
            attacker.GameObject.transform.position.z);
        
        //타겟 위치
        Vector2 targetPos = Vector2.zero;
        
        int damage = 0;
        
        foreach (var target in targets)
        {
            Tile tile = TileManager.Instance.GetCurrentTileByIDamageAble(target);
            targetPos = new Vector2(target.GameObject.transform.position.x, target.GameObject.transform.position.z);
            damage = (int)(amount * GetRelativeAttackAngle(attackerPos, targetPos, tile));
            Debug.Log($"Total Damage: {amount}, Damage: {damage}");
            var combatEvent = new CombatEvent
            {
                Sender = attacker,
                Target = target,
                Damage = damage,
            };
        
            target.TakeDamage(combatEvent);
        }
    }
    
    public void ApplyHeal(IDamageAble healer, List<IDamageAble> targets, int amount)
    {
        foreach (var target in targets)
        {
            var healEvent = new HealEvent
            {
                Sender = healer,
                Target = target,
                Heal = amount,
                Position = target.GameObject.transform.position
            };

            target.TakeHeal(healEvent);
        }
    }
    
    public void ApplyBuff(IDamageAble healer, List<IDamageAble> targets, int amount)
    {
        foreach (var target in targets)
        {
            var buffEvent = new BuffEvent
            {
                Sender = healer,
                Target = target,
                Buff = amount,
                Position = target.GameObject.transform.position
            };

            target.TakeBuff(buffEvent);
        }
    }
}
