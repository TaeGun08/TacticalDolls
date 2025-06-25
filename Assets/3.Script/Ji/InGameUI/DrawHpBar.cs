using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Shapes {

    public class DrawHpBar : ImmediateModePanel {

        public UnitParent unit { get; set; }
        private Tween widthTween;
        private Tween widthBackGroundTween;
        private bool isOnUI { get; set; }= true;
        
        private float fillAmount = 1;
        private float fillBackGroundAmount = 1;
        public Gradient allyColorGradient;
        public Gradient enemyColorGradient;
        
        private Gradient selectedColorGradient;
        private string unitName = "UnitName";

        public override void DrawPanelShapes( Rect rect, ImCanvasContext ctx ) {
            if( selectedColorGradient == null || unit == null || isOnUI == false)
                return; // just in case it hasn't initialized

            // Draw black background:
            Draw.Rectangle( rect, 8f, Color.black );
            
            // Draw the colored bar:
            Rect fillRect = Inset( rect, 8 ); // inset the rect a little bit, to give it some margin
            Rect fillBackGroundRect = Inset( rect, 8 );

            fillBackGroundRect.width *= fillBackGroundAmount;
            Draw.Rectangle( fillBackGroundRect, Color.white );
            
            fillRect.width *= fillAmount;
            Draw.Rectangle( fillRect, selectedColorGradient.Evaluate( fillAmount ) );
            
            // Draw white border:
            Draw.RectangleBorder( rect, 2f, 8f, Color.white );

            // Draw the title
            Draw.FontSize = 240;
            Vector2 topLeft = new Vector2( rect.xMin + 6f, rect.yMax + 6f );
            Draw.Text( topLeft, unitName, TextAlign.BaselineLeft );
        }

        Rect Inset( Rect r, float amount ) {
            return new Rect( r.x + amount, r.y + amount, r.width - amount * 2, r.height - amount * 2 );
        }

        public void SetUpHpBar( UnitParent unitParent )
        {
            unit = unitParent;
            unit.OnHpChanged += OnHealthChanged;
            unitName = unit.PrefabName;

            if (unit.Team == 0) //아군
            {
                selectedColorGradient =  allyColorGradient;
            }
            else
            {
                selectedColorGradient =  enemyColorGradient;
            }
        }
        
        private void OnHealthChanged()
        {
            float percent = Mathf.Clamp01(unit.Stat.HP / (float)unit.Stat.MaxHP);
        
            if (widthTween != null && widthTween.IsActive())
                widthTween.Kill();

            widthTween = DOTween.To(() => fillAmount,
                    x => fillAmount = x,
                    percent,
                    0.5f) // 애니메이션 시간 (초)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    StopCoroutine(Sleep(percent));
                    StartCoroutine(Sleep(percent));
                });//Kill되면 OnComplete되지 않으므로, 연속으로 hp가 감소하면 빨간색이 전부 줄어든 다음 흰색이 줄어들게
        }

        private IEnumerator Sleep(float targetWidth)
        {
            yield return new WaitForSeconds(0.5f);
        
            if (widthBackGroundTween != null && widthBackGroundTween.IsActive())
                widthBackGroundTween.Kill();

            widthBackGroundTween = DOTween.To(() => fillBackGroundAmount,
                    x => fillBackGroundAmount = x,
                    targetWidth,
                    0.5f) // 애니메이션 시간 (초)
                .SetEase(Ease.OutCubic);
        }
    }
}