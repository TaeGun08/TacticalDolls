using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;

public class InGameSetUpPanel : MonoBehaviour
{
    #region Field
    
    //Components
    [field: SerializeField] private ButtonManager GameStartButtonManager { get; set; }
    [field: SerializeField] private Button AutoButton { get; set; }
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
    
        public void InteractableChangeCallBacked(object sender, InGameUnitSetEventArgs e) // 게임 스타트 버튼 활성화 CallBack
        {
            GameStartButtonManager.Interactable(e.SetUpGameCharacter);
        }
        
        // 게임 시작
        public void StartGameButtonClicked()
        {
            AutoButton.gameObject.SetActive(true);
            GameStartButtonManager.gameObject.SetActive(false);
            TileManager.Instance.combatScript.SetActive(true);
            RangeSystem.Instance.ResetAllTiles();
            GameManager.Instance.SelectedCharacterPanel.SetActive(false);
        
            GameManager.Instance.UnitInitializeStarSetting();
            InGameUIEventTerminal.GameStartAction?.Invoke();
        }
    #endregion
}
