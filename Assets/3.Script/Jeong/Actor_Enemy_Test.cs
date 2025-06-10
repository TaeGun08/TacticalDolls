using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Enemy_Test : Actor_Test
{
    public override void MyTurn(Actor_Test[] actor)
    {
        gridBehavior.Actor = transform;
        
    }
}
