using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using UnityEngine;


public class InGameUIManager : MonoBehaviour
{
    #region Field
    
        //Components
        [field: SerializeField] private InGameUnitInfoPanel inGameUnitInfoPanel { get; set; }
        [field: SerializeField] private InGameHpBarPanel InGameHpBarPanel { get; set; }
        [field: SerializeField] private UnitParent testInfoUICharacter { get; set; }
    
    #endregion
    
    #region LifeCycle

        private void Start()
        {
            InGameUIEventTerminal.UnitInfoEvents.ShowUnitInfoEventHandler += ShowUnitInfoCallBacked;
            InGameUIEventTerminal.UnitInfoEvents.DisableUnitInfoAction += DisableUnitInfoCallBacked;
        }

        private void OnDisable()
        {
            InGameUIEventTerminal.UnitInfoEvents.ShowUnitInfoEventHandler -= ShowUnitInfoCallBacked;
            InGameUIEventTerminal.UnitInfoEvents.DisableUnitInfoAction -= DisableUnitInfoCallBacked;
        }
    
    #endregion
    
    #region Method
    
        #region Action
        
        private void ShowUnitInfoCallBacked(object o, ShowUnitInfoEventArgs e) //캐릭터를 클릭했을 때
        {
            Debug.Log("ShowUnitInfoCallBacked");
            inGameUnitInfoPanel.SetInGameUnitInfoPanel(e.Unit);
            inGameUnitInfoPanel.gameObject.SetActive(true);
        }
        
        private void DisableUnitInfoCallBacked() // 캐릭터가 아닌 다른 곳을 클릭했을 때
        {
            inGameUnitInfoPanel.SetInGameUnitInfoPanel(null);
            inGameUnitInfoPanel.gameObject.SetActive(false);
        }
        #endregion
    
    #endregion
}
