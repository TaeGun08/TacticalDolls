using System;
using UnityEngine;

#region EventArgs
public class ShowUnitInfoEventArgs : EventArgs
{
    public UnitParent Unit { get; }
    public ShowUnitInfoEventArgs(UnitParent unit) => Unit = unit;
}

public class InGameUnitSetEventArgs : EventArgs 
{
    public bool SetUpGameCharacter { get; set; }
    public InGameUnitSetEventArgs(int count)
    {
        SetUpGameCharacter = count > 0;
    }
}

#endregion

#region EventHandlers

public class InGameUIEventTerminal : MonoBehaviour
{
    public static EventHandler<ShowUnitInfoEventArgs> ShowUnitInfoEventHandler; //유닛을 클릭할 때 수신
    public static EventHandler<InGameUnitSetEventArgs> SetInGameUnitEventHandler; //유닛을 맵에 배치할 때 수신
    
    public static Action DisableUnitInfoAction;
}

#endregion
