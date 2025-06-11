using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor_Test : MonoBehaviour
{
    protected GridBehavior_Test gridBehavior;
    protected Turn_Test turn;

    protected virtual void Start()
    {
        gridBehavior = GridBehavior_Test.Instance;
        turn = Turn_Test.Instance;
    }

    public abstract void MyTurn();
}
