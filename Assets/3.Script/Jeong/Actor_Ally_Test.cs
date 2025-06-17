using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Ally_Test : Actor_Test
{
    private CharacterData characterData;
    public override IDamageAble DamageAble => characterData;

    protected void Awake()
    {
        characterData = GetComponent<CharacterData>();
    }

    protected override void Start()
    {
        base.Start();
        turn.Ally.Add(this);
    }

    public override void Excute(int selectedSkill)
    {
        float distance = 1000f;
        Vector3Int pos = PathFindingManager.Instance.RoundToTilePosition(transform.position);
        IDamageAble targetAble = null;
        switch (ActorPosition)
        {
            case ActorPosition.Shooter:
                foreach (var enemy in turn.Enemy)
                {
                    if (distance > Vector3.Distance(pos,
                            PathFindingManager.Instance.RoundToTilePosition(enemy.transform.position)))
                    {
                        distance = Vector3.Distance(pos, 
                            PathFindingManager.Instance.RoundToTilePosition(enemy.transform.position));
                        targetAble = enemy.DamageAble;    
                    }
                }
                break;
            case ActorPosition.Supporter:
                foreach (var ally in turn.Ally)
                {
                    if (ally == this) continue;
                    if (distance > Vector3.Distance(pos,
                            PathFindingManager.Instance.RoundToTilePosition(ally.transform.position)))
                    {
                        distance = Vector3.Distance(pos, 
                            PathFindingManager.Instance.RoundToTilePosition(ally.transform.position));
                        targetAble = ally.DamageAble;    
                    }
                }
                break;
            case ActorPosition.Vanguard:
                foreach (var enemy in turn.Enemy)
                {
                    if (distance > Vector3.Distance(pos,
                            PathFindingManager.Instance.RoundToTilePosition(enemy.transform.position)))
                    {
                        distance = Vector3.Distance(pos, 
                            PathFindingManager.Instance.RoundToTilePosition(enemy.transform.position));
                        targetAble = enemy.DamageAble;    
                    }
                }
                break;
            case ActorPosition.Enemy:
                break;
        }
        
        
        combatSystem.ExecuteSkill(DamageAble, targetAble, selectedSkill);
    }
}
