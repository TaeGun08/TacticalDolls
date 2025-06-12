using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Ally_Test : Actor_Test
{
    protected override void Start()
    {
        base.Start();
        turn.Ally.Add(this);
    }
    
    public override void MyTurn()
    {
        if (turn.IsAuto)
        {
            gridBehavior.Actor = this;
            gridBehavior.AllyAutoMove();
        }
        else
        {
            gridBehavior.AllyInputMove();
        }
    }
}
