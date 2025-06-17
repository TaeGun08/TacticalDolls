using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor_Enemy_Test : Actor_Test
{
    private EnemyData enemyData;
    public override IDamageAble DamageAble => enemyData;

    protected void Awake()
    {
        enemyData = GetComponent<EnemyData>();
    }
    
    protected override void Start()
    {
        base.Start();
        turn.Enemy.Add(this);
    }

    public override void Excute(int selectedSkill)
    {
        float distance = 1000f;
        Vector3Int pos = PathFindingManager.Instance.RoundToTilePosition(transform.position);
        IDamageAble targetAble = null;
        switch (ActorPosition)
        {
            case ActorPosition.Shooter:
                break;
            case ActorPosition.Supporter:
                break;
            case ActorPosition.Vanguard:
                break;
            case ActorPosition.Enemy:
                foreach (var ally in turn.Ally)
                {
                    if (distance > Vector3.Distance(pos,
                            PathFindingManager.Instance.RoundToTilePosition(ally.transform.position)))
                    {
                        distance = Vector3.Distance(pos, 
                            PathFindingManager.Instance.RoundToTilePosition(ally.transform.position));
                        targetAble = ally.DamageAble;    
                    }
                }
                break;
        }
        
        combatSystem.ExecuteSkill(DamageAble, targetAble, selectedSkill);
    }
}
