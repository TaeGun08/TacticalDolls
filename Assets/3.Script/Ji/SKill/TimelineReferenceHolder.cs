// #if UNITY_EDITOR
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Playables;
// using UnityEngine.SceneManagement;
// using UnityEngine.Timeline;
//
// [InitializeOnLoad]
// public static class ChildAttachmentWatcher
// {
//     static ChildAttachmentWatcher()
//     {
//         EditorApplication.hierarchyChanged += OnHierarchyChanged;
//     }
//     
//     /// <summary>
//     /// 사용 방법 :
//     /// 1. 타임라인에 원하는 만큼 트랙을 만드세요. TransformTweenTrack 또는 CustomBezierCurveTweenTrack에서만 작동합니다.
//     /// 
//     /// 2. 원하는 순서대로 배치하세요. 바인딩되는 순서는 트랙의 순서를 따릅니다.
//     /// 
//     /// 3. 바꿔끼울 오브젝트들(파티클)의 부모를 만드세요. 그 후 사용할 오브젝트를 자식으로 '하나'만 생성해주세요.
//     /// 
//     /// 4. 첫번째 트랙에서 클립을 하나 만들고, 인스펙터에서 3번 과정에서 만든 오브젝트를 클립에 모두 할당합니다. 또한 다른 빈 컴포넌트도 채워주세요.
//     /// 
//     /// 5. 원하는 수 만큼 트랙을 복제합니다.
//     /// 
//     /// 6. 씬, 스킬, 부모의 이름을 아래 스크립트와 일치하게 설정해주세요.
//     /// 
//     /// 7. 3번 과정에서 만든 부모의 자식들을 트랙 갯수만큼 '복제'합니다. 트랙에 자동으로 복사한 자식 오브젝트들이 순서대로 할당됩니다.
//     ///
//     /// 8. 아래 주의사항을 확인하세요.
//     /// </summary>
//
//     // 주의사항 1 ::: 만들기 전에 작업할 씬과 변경을 원하는 스킬의 '이름'을 제대로 확인해주세요. 여러 개의 스킬을 만들 때 덮어씌워질 가능성이 있습니다.
//     
//     // 주의사항 2 ::: 바꿔 끼울 클립의 컴포넌트들은 None 이어선 안됩니다.
//     // 실수로 None 인채로 위 과정을 따라했을 경우에는 정상적으로 할당되지 않으니, 다음 과정을 진행하세요. 또는 지하랑한테 짬때리기
//     // 빈 오브젝트를 만들어, 정상적으로 할당되지 않은 컴포넌트에 할당해주시고 3번 과정에서 만든 부모의 자식을 새로 복제합니다.
//     // 정상적으로 할당되었다면 빈 오브젝트를 삭제합니다.
//     
//
//     
//     static void OnHierarchyChanged()
//     {
//         var activeScene = SceneManager.GetActiveScene(); //현재 위치한 씬을 받아옵니다.
//         
//         //작업할 씬 이름을 넣어주세요.
//         if (activeScene.name != "None")
//         {
//             EditorApplication.hierarchyChanged -= OnHierarchyChanged;
//             return;
//         }
//         
//         //PlayableDirector 컴포넌트를 가진 스킬의 이름을 넣어주세요.
//         PlayableDirector director = GameObject.Find("ShooterSkill 1").GetComponent<PlayableDirector>();
//         if(director == null) return;
//         
//         // 부모로 설정하고 싶은 기준 오브젝트 이름을 넣어주세요.
//         //자식이 가진 탄환, 파티클을 불러옵니다.
//         var projectileParent = GameObject.Find("Projectiles");
//         var flashParticleParent = GameObject.Find("FlashParticles");
//         var hitParticleParent = GameObject.Find("HitParticles");
//         
//         if (!projectileParent || !flashParticleParent || !hitParticleParent)
//             return;
//
//         Transform[] childProjectiles = ReturnChild(projectileParent);
//         Transform[] childFlashParticles = ReturnChild(flashParticleParent);
//         Transform[] childHitParticles = ReturnChild(hitParticleParent);
//         
//         TryBindToTimeline(director,childProjectiles, childFlashParticles, childHitParticles);
//     }
//
//     static Transform[] ReturnChild(GameObject parent)
//     {
//         int count = parent.transform.childCount;
//         
//         Transform[] childrens = new Transform[count];
//         
//         for (int i = 0; i < count; i++)
//         {
//             childrens[i] = parent.transform.GetChild(i);
//         }
//
//         return childrens;
//     }
//     
//     static void TryBindToTimeline(PlayableDirector director, Transform[] childProjectiles, Transform[] childFlashParticles, Transform[] childHitParticles)
//     {
//         if (!director || !(director.playableAsset is TimelineAsset timeline))
//             return;
//
//         int index = 0;
//         foreach (var track in timeline.GetOutputTracks())
//         {
//             if (index >= childProjectiles.Length) break;
//             if (track is not (TransformTweenTrack or CustomBezierCurveTweenTrack)) continue;
//
//             director.SetGenericBinding(track, childProjectiles[index]);
//
//             
//             var targetClip = track.GetClips().First().asset;
//             
//             bool isOutOfRangeFlash = index < childFlashParticles.Length;
//             bool isOutOfRangeHit = index < childHitParticles.Length;
//             
//             if (targetClip is TransformTweenClip tweenClip) //형변환
//             {
//                 if(isOutOfRangeFlash)
//                     director.SetReferenceValue(tweenClip.flashParticle.exposedName, childFlashParticles[index].gameObject);
//                 
//                 if(isOutOfRangeHit)
//                     director.SetReferenceValue(tweenClip.hitParticle.exposedName, childHitParticles[index].gameObject);
//             }
//             
//             if (targetClip is CustomBezierCurveTweenClip bezierCurveTweenClip) //형변환
//             {
//                 if(isOutOfRangeFlash)
//                     director.SetReferenceValue(bezierCurveTweenClip.flashParticle.exposedName, childFlashParticles[index].gameObject); 
//                 
//                 if(isOutOfRangeHit)
//                     director.SetReferenceValue(bezierCurveTweenClip.hitParticle.exposedName, childHitParticles[index].gameObject);
//             }
//             
//             index++;
//         }
//         Debug.Log("바인딩 완료");
//     }
// }
// #endif