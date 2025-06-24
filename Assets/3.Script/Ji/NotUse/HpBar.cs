// using System;
// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using Shapes;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// public class HpBar : MonoBehaviour
// {
//     public UnitParent unit;
//     public Line hpBar; // Shapes Rectangle 참조
//     public Rectangle hpBackGroundBar;
//     
//     private float maxHpWidth;
//     private float maxHpWidthBackGround;
//     
//     private Tween widthTween;
//     private Tween widthBackGroundTween;
//     private Camera mainCam;
//     
//     void Start()
//     {
//         mainCam = Camera.main;
//         // maxHpWidth = hpBar.Width;
//         maxHpWidthBackGround = hpBackGroundBar.Width;
//         // unit.OnHpChanged += OnHealthChanged;
//         Debug.Log($" unit attack{unit.Stat.Attack}");
//     }
//
//     void LateUpdate()
//     {
//         Vector3 pivot = Vector3.zero; // 회전 중심점
//         transform.RotateAround(pivot, Vector3.up, 45f * Time.deltaTime);
//         
// //         if (Input.GetKeyDown(KeyCode.Alpha3))
// //         {
// // // 목표 회전
// //             Quaternion targetRot = Quaternion.Euler(0, 90f, 0);
// //
// // // 매 프레임 부드럽게 회전
// //             transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 0.5f);
// //         }
//     }
//     
//     public void OnHealthChanged()
//     {
//         Debug.Log($" {unit.Stat.HP} / {unit.Stat.MaxHP}");
//         float percent = Mathf.Clamp01(unit.Stat.HP / (float)unit.Stat.MaxHP);
//         float targetWidth = maxHpWidth * percent;
//         float targetwidthBackGround = maxHpWidthBackGround * percent;
//         
//         if (widthTween != null && widthTween.IsActive())
//             widthTween.Kill();
//         //
//         // widthTween = DOTween.To(() => hpBar.Width,
//         //         x => hpBar.Width = x,
//         //         targetWidth,
//         //         2f) // 애니메이션 시간 (초)
//         //     .SetEase(Ease.OutCubic);
//         
//         StopCoroutine(Sleep(targetwidthBackGround));
//         StartCoroutine(Sleep(targetwidthBackGround));
//     }
//
//     private IEnumerator Sleep(float targetWidth)
//     {
//         yield return new WaitForSeconds(2f);
//         
//         if (widthBackGroundTween != null && widthBackGroundTween.IsActive())
//             widthBackGroundTween.Kill();
//         
//         widthBackGroundTween = DOTween.To(() => hpBackGroundBar.Width,
//                 x => hpBackGroundBar.Width = x,
//                 targetWidth,
//                 2f) // 애니메이션 시간 (초)
//             .SetEase(Ease.OutCubic);
//     }
// }