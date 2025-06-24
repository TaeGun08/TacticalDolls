using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class HpBarPack
{
    public Transform targetTransform { get; set; }
    public DrawHpBar drawHpBar { get; set; }
    public RectTransform drawHpBarRectTransform { get; set; }
}

public class InGameHpBarManager : MonoBehaviour
{
    //ingame
    public Camera targetCamera;
    public Canvas parentCanvas; 
    public DrawHpBar drawHpBarPrefab;
    
    private HpBarPack[] hpBarPacks;
    private List<DrawHpBar> hpBars;
    private bool initialized = false;
    
    private void Awake()
    {
        hpBars = new List<DrawHpBar>();
        hpBars.AddRange(gameObject.GetComponentsInChildren<DrawHpBar>(true)); // 비활성 포함하여 풀링
    }

    private void Start()
    {
        GameManager.Instance.GameStartAction += OnSetUpHpBar;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameStartAction -= OnSetUpHpBar;
    }

    private void Update()
    {
        if( initialized == false ) return;

        foreach (var t in hpBarPacks) //매 프레임마다 위치 동기화
        {
            if (t.drawHpBar.unit.Stat.IsDead)
            {
                t.drawHpBar.gameObject.SetActive(false);
                t.drawHpBar = null;
                continue;
            }
            
            t.drawHpBar.gameObject.SetActive(true);
            
            Vector3 screenPoint = targetCamera.WorldToScreenPoint(t.targetTransform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                parentCanvas.transform as RectTransform, 
                screenPoint,
                targetCamera, 
                out Vector2 localPoint
            );

            t.drawHpBarRectTransform.anchoredPosition = localPoint;
        }
    }
    
    private void OnSetUpHpBar() //Hp Bar 세팅
    {
        //나중에 최적화
        List<UnitParent> targets = new List<UnitParent>();
        targets.AddRange(GameManager.Instance.PlayerUnits);
        targets.AddRange(GameManager.Instance.EnemyUnits);
        
        hpBarPacks = new HpBarPack[targets.Count];
        
        if (targets.Count > hpBars.Count)
        {
            CreateHpBarInstances(targets.Count - hpBars.Count); //동적 생성
        }
        
        int hpBarPacksIndex = 0;
        
        for (int i = 0; i < targets.Count; i++)
        {
            hpBarPacks[i].targetTransform = targets[i].hpBarTransform; //유닛의 HpBar 트랜스폼 (할당 필요)
            hpBarPacks[i].drawHpBarRectTransform = hpBars[i].transform as RectTransform; //RectTransform 형변환
            
            hpBarPacks[i].drawHpBar = hpBars[i];
            hpBarPacks[i].drawHpBar.SetUpHpBar(targets[i]);
            hpBarPacks[i].drawHpBar.gameObject.SetActive(true);
            Debug.Log("1111111111111111111111");
            
            hpBarPacksIndex ++;
        }
        
        // hpBarPacksIndex가 끝난 지점부터 ~ 사용하지 않는 오브젝트들을 끕니다.
        for (int i = hpBarPacksIndex; i < hpBars.Count; i++)
        {
            hpBars[i].gameObject.SetActive(false);
        }
        
        initialized =  true;
    }
    
    private void CreateHpBarInstances(int count)
    {
        for (int i = 0; i < count; i++)
        {
            DrawHpBar hpBar = Instantiate(drawHpBarPrefab, gameObject.transform);
            hpBar.gameObject.SetActive(false);
            hpBars.Add(hpBar);
        }
    }
}
