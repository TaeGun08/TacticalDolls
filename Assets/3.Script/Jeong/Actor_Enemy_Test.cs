using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor_Enemy_Test : Actor_Test
{
    private EnemyData enemyData;

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
        combatSystem.ExecuteSkill(enemyData, selectedSkill);
    }
}
