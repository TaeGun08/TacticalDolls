using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUnitInfoPanel : MonoBehaviour
{
    #region Field
    
    [SerializeField] private Image unitImage;
    [SerializeField] private DrawHpBar unitHpBar;
    #endregion
    
    #region Method
        public void SetInGameUnitInfoPanel(UnitParent unitParent) //유닛 정보 표시 왼쪽 하단
        {
            unitHpBar.SetUpHpBar(unitParent);
            
            if (!unitParent)
            {
                unitHpBar.gameObject.SetActive(false); //null이라면 꺼주기
            }
            else
            {
                unitImage.sprite = unitParent.CharacterIcon;
                unitHpBar.gameObject.SetActive(true); //아니라면 켜주기
            }
        }
    #endregion
}
