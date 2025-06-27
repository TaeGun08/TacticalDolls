using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using UnityEngine;



public class InGameSetUpPanel : MonoBehaviour
{
    #region Field
    
    //Components
    [field: SerializeField] private GameManager GameManager { get; set; }
    [field: SerializeField] private ButtonManager GameStartButtonManager { get; set; }
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        InGameUIEventTerminal.SetInGameUnitEventHandler += InteractableChangeCallBacked;
    }

    private void OnDisable()
    {
        InGameUIEventTerminal.SetInGameUnitEventHandler -= InteractableChangeCallBacked;
    }

    #region Action
    
        public void InteractableChangeCallBacked(object sender, InGameUnitSetEventArgs e) // 게임 스타트 버튼 CallBack
        {
            GameStartButtonManager.Interactable(e.SetUpGameCharacter);
        }
        
    #endregion
}
