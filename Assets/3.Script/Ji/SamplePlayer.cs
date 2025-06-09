using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePlayer : MonoBehaviour
{
    public void GetTurn(Action turn = null)
    {
        
        turn?.Invoke();
    }

    public void Excute()
    {
        
    }
}
