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
            InGameUIEventTerminal.ShowUnitInfoEventHandler += ShowUnitInfoCallBacked;
            InGameUIEventTerminal.DisableUnitInfoAction += DisableUnitInfoCallBacked;
        }

        private void OnDisable()
        {
            InGameUIEventTerminal.ShowUnitInfoEventHandler -= ShowUnitInfoCallBacked;
            InGameUIEventTerminal.DisableUnitInfoAction -= DisableUnitInfoCallBacked;
        }
    
    #endregion
    
    #region Method
    
        #region Action
        
        private void ShowUnitInfoCallBacked(object o, ShowUnitInfoEventArgs e) //캐릭터를 클릭했을 때
        {
            inGameUnitInfoPanel.gameObject.SetActive(true);
            inGameUnitInfoPanel.SetInGameUnitInfoPanel(e.Unit);
        }
        
        private void DisableUnitInfoCallBacked() // 캐릭터가 아닌 다른 곳을 클릭했을 때
        {
            inGameUnitInfoPanel.gameObject.SetActive(false);
        }
        #endregion
    
    #endregion
}
