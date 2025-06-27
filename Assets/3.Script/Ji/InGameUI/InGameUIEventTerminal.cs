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
        public class UnitInfooCallBack
        {
            public EventHandler<ShowUnitInfoEventArgs> ShowUnitInfoEventHandler; //유닛을 클릭할 때 수신
            public Action DisableUnitInfoAction; //유닛이 아닌 다른 것을 클릭했을 때 수신
        }
        public static UnitInfooCallBack UnitInfoEvents;
        
        public static EventHandler<InGameUnitSetEventArgs> SetInGameUnitEventHandler; //유닛을 맵에 배치할 때 수신
        

    }

#endregion
