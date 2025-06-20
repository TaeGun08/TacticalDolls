
//타임라인 참고용. 더이상 사용하지 않습니다.

// public class DynamicTransformTweenChanger : MonoBehaviour
// {
//     public static DynamicTransformTweenChanger Instance;
//     
//     private const string MOVE_TWEEN_TRACK = "Move Tween Track";
//     private const string FIRE_PROJECTILE_TRACK = "Fire Projectile Track";
//     private const string CUSTOM_BEZIER_CURVE_TWEEN_TRACK = "Custom Bezier Curve Tween Track";
//     
//     // public PlayableDirector director;         // 타임라인을 실행 중인 PlayableDirector
//     public Transform testplayer;
//     public Transform shotTransform;
//     
//     private PlayableAsset cashTweenClip;
//     private string targetTrack;
//
//     public PlayableDirector testDirector;
//     public TimelineAsset Target;
//     public TimelineAsset NewSourceTimelineAsset;
//     public CameraPerspective touchCamera;
//     
//     private void Awake()
//     {
//         Instance = this;
//     }
//
//     private void Update()
//     {
//         // if (Input.GetKeyDown(KeyCode.Space))
//         // {
//         //
//         // }
//         
//         if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭 또는 모바일 터치
//         {
//             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             RaycastHit hit;
//
//             if (Physics.Raycast(ray, out hit))
//             {
//                 // Debug.Log("Hit Point: " + hit.point);
//
//                 // 충돌 지점의 좌표로 Transform 생성 (예시)
//                 GameObject marker = new GameObject("HitPointTransform");
//                 marker.transform.position = hit.point;
//                 marker.transform.rotation = Quaternion.identity;
//
//                 // 필요 시 marker.transform으로 다른 연산 가능
//                 
//                 ChangeTransformTweenLocation(testDirector, null, marker.transform);
//                 
//                 CopyClipsFromTo(NewSourceTimelineAsset, Target);
//                 
//                 ChangeBezierTweenLocation(testDirector, testplayer, shotTransform);
//             
//                 testDirector.Play();
//                 // Debug.Log(testplayer.position);
//                 // CameraEvents.OnRequestObjectFocus?.Invoke(testplayer.gameObject, false);
//                 
//                 Quaternion targetRotation = Quaternion.Euler(45f, 180f, 0f);
//                 touchCamera.MoveCameraTo(targetRotation);
//             }
//         }
//     }
//
//     //클립의 로케이션 변경
//     public void ChangeTransformTweenLocation(PlayableDirector director, Transform newStartLocation = null, Transform newEndLocation = null)
//     {
//         cashTweenClip = FindClipByType<TransformTweenTrack>(director);
//         
//         if (cashTweenClip is TransformTweenClip tweenClip) //형변환
//         {
//             // 새 Location을 PlayableDirector에 등록
//             //start필요없긴하지만 테스트용으로
//             if(newStartLocation != null)
//                 director.SetReferenceValue(tweenClip.startLocation.exposedName, newStartLocation); //동적할당 하기 위해선 exposedName사용
//             director.SetReferenceValue(tweenClip.endLocation.exposedName, newEndLocation);
//         }
//         
//         // 변경사항 적용
//         director.RebuildGraph();
//     }
//     
//     public void ChangeBezierTweenLocation(PlayableDirector director, Transform newStartLocation = null, Transform newEndLocation = null)
//     {
//         cashTweenClip = FindClipByType<CustomBezierCurveTweenTrack>(director) as CustomBezierCurveTweenClip;
//         
//         if (cashTweenClip is CustomBezierCurveTweenClip tweenClip) //형변환
//         {
//             // 새 Location을 PlayableDirector에 등록
//             if(newStartLocation != null)
//                 director.SetReferenceValue(tweenClip.startLocation.exposedName, newStartLocation); //동적할당 하기 위해선 exposedName사용
//             director.SetReferenceValue(tweenClip.endLocation.exposedName, newEndLocation);
//         }
//         
//         // 변경사항 적용
//         director.RebuildGraph();
//     }
//     
//     public void ChangeTweenLocation<T>(PlayableDirector director, Transform newStartLocation = null, Transform newEndLocation = null)
//     {
//         cashTweenClip = FindClipByType<CustomBezierCurveTweenTrack>(director) as CustomBezierCurveTweenClip;
//         
//         if (cashTweenClip is CustomBezierCurveTweenClip tweenClip) //형변환
//         {
//             // 새 Location을 PlayableDirector에 등록
//             if(newStartLocation != null)
//                 director.SetReferenceValue(tweenClip.startLocation.exposedName, newStartLocation); //동적할당 하기 위해선 exposedName사용
//             director.SetReferenceValue(tweenClip.endLocation.exposedName, newEndLocation);
//         }
//         
//         // 변경사항 적용
//         director.RebuildGraph();
//     }
//     
//     private PlayableAsset FindClipByType<T>(PlayableDirector director) //탄환 도착 지점 동적할당
//     {
//         TimelineAsset timeline = director.playableAsset as TimelineAsset;
//
//         if (!timeline)
//         {
//             Debug.LogError(" missing timeline asset");
//             return null;
//         }
//         
//         foreach (TrackAsset track in timeline.GetOutputTracks()) //트랙 찾기
//         {
//             // if(track.name != targetTrack) continue; //이름으로 트랙 찾기
//             if(track.GetType() != typeof(T)) continue;
//             
//             foreach (TimelineClip clip in track.GetClips()) //클립 찾기
//             {
//                 return clip.asset as PlayableAsset; //일반화
//             }
//         }
//         
//         return null;
//     }
//     
//     
//     public void CopyClipsFromTo(TimelineAsset newSource, TimelineAsset target)
//     {
//         //최적화 필요
//         foreach (TrackAsset oldTrack in target.GetOutputTracks())
//         {
//             foreach (var oldClip in oldTrack.GetClips())
//             {
//                 foreach (var newSourceTrack in newSource.GetOutputTracks())
//                 {
//                     foreach (var newClip in newSourceTrack.GetClips())
//                     {
//                         if (oldClip.asset.GetType() == newClip.asset.GetType())
//                         {
//                             // Debug.Log("oldClipChange");
//                             oldClip.start = newClip.start;
//                             oldClip.duration = newClip.duration;
//                             oldClip.displayName = newClip.displayName;
//                             oldClip.asset = newClip.asset;
//                         }
//                     }
//                 }
//             }
//         }
//         
//         testDirector.RebuildGraph();
//         
//         // foreach (var track in target.GetOutputTracks())
//         // {
//         //     if (track is GroupTrack groupTrack)
//         //     {
//         //         Debug.Log("Group: " + groupTrack.name);
//         //         foreach (var child in groupTrack.GetChildTracks())
//         //         {
//         //             Debug.Log($"  └── Child Track: {child.name}");
//         //         }
//         //     }
//         // }
//         //
//         // foreach (TrackAsset oldTrack in target.GetOutputTracks())
//         // {
//         //     foreach (var newSourceTrack in newSource.GetOutputTracks())
//         //     {
//         //         if(oldTrack.GetType() != newSourceTrack.GetType()) continue;
//         //
//         //         foreach (TimelineClip oldClip in oldTrack.GetClips())
//         //         {
//         //             TimelineClip newClip = newSourceTrack.GetClips().First();
//         //             
//         //             oldClip.start = newClip.start;
//         //             oldClip.duration = newClip.duration;
//         //             oldClip.displayName = newClip.displayName;
//         //             oldClip.asset = newClip.asset;
//         //         }
//         //     }
//         // }
//         
//         // return Task.CompletedTask;
//         //
//         // Debug.Log("endLocation SetReferenceValue 적용 완료");
//         //
//         // testDirector.Play();
//     }
//     
//     public float ReturnAStarDuration(List<Vector2Int> path)
//     {
//         float totalDistance = 0f;
//         for (int i = 1; i < path.Count; i++)
//         {
//             totalDistance += Vector2Int.Distance(path[i - 1], path[i]);
//         }
//         
//         float speed = 2.0f; // 고정된 이동 속도 (예: 2m/s)
//         float duration = totalDistance / speed;
//         
//         return duration;
//     }
// }