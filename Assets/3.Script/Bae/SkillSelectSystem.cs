using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectSystem : MonoBehaviour
{
    public static SkillSelectSystem Instance;
    
    public GameObject panel;
    public Button[] skillButtons = new Button[4];
    public TMP_Text[] skillNameTexts = new TMP_Text[4];

    public Button cancelButton;
    public Button selectButton;
    
    public bool IsSelectingSkill { get; set; }

    private IDamageAble currentTarget;

    private void Awake()
    {
        Instance = this;
    }

    public int currentSkill;

    private void Start()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            currentSkill = i;
            skillButtons[i].onClick.AddListener(() => OnSkillButtonClicked(currentSkill));
        }
        
        cancelButton.onClick.AddListener(() =>
        {
            IsSelectingSkill = false;
            
            GameManager.Instance.EndTurnBtn.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(false);
            
            GameManager.Instance.CurrentEnemy = null;
            
            RangeSystem.Instance.ResetAllTiles();
            RangeSystem.Instance.ShowMoveRange(
                TileManager.Instance.GetCurrentTileByIDamageAble(currentTarget), 
                currentTarget.Stat.MoveRange);
        });
        

        selectButton.onClick.AddListener(async () =>
        {
            cancelButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(false);
            RangeSystem.Instance.ResetAllTiles();
            
            IsSelectingSkill = false;
            
            Debug.Log($"target :: {currentTarget}, selectskill :: {currentSkill}");
            
            Debug.Log("StartMove");
            if (GameManager.Instance.MoveChoiceTile != null)
            {
                GridBehavior.Instance.Actor = currentTarget;
                List<Node> path = PathFindingManager.Instance.PathFind(currentTarget.GameObject.transform.position, GameManager.Instance.MoveChoiceTile.transform.position);
                await GridBehavior.Instance.MovePlayerAlongPath(path, Vector3.zero);
            }
            
            Debug.Log("EndMove");
            
            // List<IDamageAble> targetList = RangeSystem.Instance.damageAbles;
            // Transform targetTransform = RangeSystem.Instance.currentTile.transform;
            
            await currentTarget.Excute(currentSkill, RangeSystem.Instance.damageAbles, RangeSystem.Instance.currentTile.transform);
            
            InintializeAfterSkillExcute();
        });
    }
    
    
    public void InintializeAfterSkillExcute()
    {
        // 스킬 사용후 초기화 되어야 할 내용
        GameManager.Instance.CurrentEnemy = null;
        GameManager.Instance.MoveChoiceTile = null;
                
        GameManager.Instance.OnCharacterEndTurn();
    }

    public void Open(IDamageAble targetData)
    {
        panel.SetActive(true);
        currentTarget = targetData;

        OpenSkills(currentTarget);
    }

    private void OpenSkills(IDamageAble unit)
    {
        for (int i = 0; i < skillNameTexts.Length; i++)
        {
            if (i < unit.Stat.Skills.Count && unit.Stat.Skills[i] != null)
            {
                skillNameTexts[i].text = unit.Stat.Skills[i].Name;
                skillButtons[i].interactable = true;

                int capturedIndex = i;
                skillButtons[i].onClick.RemoveAllListeners();
                skillButtons[i].onClick.AddListener(() => OnSkillButtonClicked(capturedIndex));
            }
        }
    }
    
    public void Close()
    {
        panel.SetActive(false);
        currentTarget = null;
    }

    private void OnSkillButtonClicked(int skillIndex)
    {
        GameManager.Instance.CurrentEnemy = null;
        
        if (skillIndex >= currentTarget.Stat.Skills.Count) return;

        SkillEffectHandlerBase skill = currentTarget.Stat.Skills[skillIndex];
        if (skill == null) return;

        GameManager.Instance.EndTurnBtn.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        selectButton.gameObject.SetActive(true);

        if (GameManager.Instance.CurrentEnemy == null)
        {
            selectButton.interactable = false;
        }
        
        IsSelectingSkill = true;
        
        Tile tempTile = GameManager.Instance.MoveChoiceTile == null
            ? TileManager.Instance.GetCurrentTileByIDamageAble(currentTarget)
            : GameManager.Instance.MoveChoiceTile;
        
        RangeSystem.Instance.ResetAllTiles();
        RangeSystem.Instance.ShowAttackRange(tempTile, currentTarget.Stat.MoveRange);  // TODO MoveRange -> AttackRange로 수정 필요

        currentSkill = skillIndex;
        // SkillRangeSystem.Instance.ClearUsableTiles();
        // SkillRangeSystem.Instance.ClearDamageAbles();
        //SkillRangeSystem.Instance.ShowSkillRange(currentTarget, ,index);

        //TestCombat(index);
    }

    // public void TestCombat(int skillIndex)
    // {
    //     Debug.Log($"skillIndex: {skillIndex}");
    //     CombatSystem.Instance.ExecuteSkill(currentTarget, skillIndex);
    // }
}
