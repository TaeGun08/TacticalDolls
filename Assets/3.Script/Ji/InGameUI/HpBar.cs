using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Shapes;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public Rectangle hpBar; // Shapes Rectangle 참조
    public Rectangle hpBackGroundBar;
    private float maxHpWidth;
    private float maxHpWidthBackGround;
    
    public IDamageAble unit;
    private Tween widthTween;
    private Tween widthBackGroundTween;
    
    void Start()
    {
        maxHpWidth = hpBar.Width;
        maxHpWidthBackGround = hpBackGroundBar.Width;
        unit.OnHpChanged += OnHealthChanged;
    }

    public void OnHealthChanged()
    {
        float percent = Mathf.Clamp01(unit.Stat.HP / (float)unit.Stat.MaxHP);
        float targetWidth = maxHpWidth * percent;
        float targetwidthBackGround = maxHpWidthBackGround * percent;
        
        if (widthTween != null && widthTween.IsActive())
            widthTween.Kill();
        
        widthTween = DOTween.To(() => hpBar.Width,
                x => hpBar.Width = x,
                targetWidth,
                2f) // 애니메이션 시간 (초)
            .SetEase(Ease.OutCubic);
        
        StopCoroutine(Sleep(targetwidthBackGround));
        StartCoroutine(Sleep(targetwidthBackGround));
    }

    private IEnumerator Sleep(float targetWidth)
    {
        yield return new WaitForSeconds(2f);
        
        if (widthBackGroundTween != null && widthBackGroundTween.IsActive())
            widthBackGroundTween.Kill();
        
        widthBackGroundTween = DOTween.To(() => hpBackGroundBar.Width,
                x => hpBackGroundBar.Width = x,
                targetWidth,
                2f) // 애니메이션 시간 (초)
            .SetEase(Ease.OutCubic);
    }
}