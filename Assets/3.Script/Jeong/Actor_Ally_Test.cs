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
    
    public override void OnMoveStart()
    {
        OnMoveEnd();
    }

    protected override void OnMoveEnd()
    {
        turn.MoveTcs.TrySetResult(true);
    }
}
