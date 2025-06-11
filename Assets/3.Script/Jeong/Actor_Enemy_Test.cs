using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor_Enemy_Test : Actor_Test
{
    protected override void Start()
    {
        base.Start();
        turn.Enemy.Add(this);
    }
    
    public override void MyTurn()
    {
        Debug.Log(gameObject.name);
        gridBehavior.Actor = this;
        gridBehavior.EnemyMove();
    }
}
