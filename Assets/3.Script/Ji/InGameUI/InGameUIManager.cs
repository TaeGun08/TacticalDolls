using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using Sirenix.Utilities.Editor;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    #region Field
    
    //Components
    [field: SerializeField]
    public GameManager gameManager { get; set; }
    
    [field: SerializeField]
    private ButtonManager buttonManager { get; set; }

    [SerializeField] private InGameUnitInfoPanel inGameUnitInfoPanel;
    
    #endregion
    
    #region LifeCycle

    private void Start()
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        throw new NotImplementedException();
    }
    
    #endregion
    
    #region Method
    
        #region Action
        
        //캐릭터를 클릭했을 때
        public void ShowUnitInfoCallBacked(ShowUnitInfoEventArgs e) // 유닛 정보
        {
            inGameUnitInfoPanel.SetInGameUnitInfoPanel(e.unit);
        }
        
        public void InteractableChangeCallBacked(bool isInteractable) // 게임 스타트 버튼 CallBack
        {
            buttonManager.Interactable(isInteractable);
        }
        
        #endregion
    
    #endregion
}
