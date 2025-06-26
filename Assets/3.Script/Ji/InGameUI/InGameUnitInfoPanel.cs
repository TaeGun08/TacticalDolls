using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowUnitInfoEventArgs : EventArgs
{
    public UnitParent unit;
    public ShowUnitInfoEventArgs(UnitParent unit) => this.unit = unit;
}


public class InGameUnitInfoPanel : MonoBehaviour
{
    #region Field
    
    [SerializeField] private RawImage unitImage;
    [SerializeField] private DrawHpBar unitHpBar;
    
    #endregion


    #region Method
    public void SetInGameUnitInfoPanel(UnitParent unitParent)
    {
        // unitImage = unitParent.icon
        unitHpBar.SetUpHpBar(unitParent);
    }
    
    #endregion

}
