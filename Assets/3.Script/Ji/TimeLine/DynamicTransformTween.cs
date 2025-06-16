using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class DynamicTransformTweenChanger : MonoBehaviour
{
    public static DynamicTransformTweenChanger Instance;
    
    private const string MOVE_TWEEN_TRACK = "Move Tween Track";
    private const string FIRE_PROJECTILE_TRACK = "Fire Projectile Track";
    private const string CUSTOM_BEZIER_CURVE_TWEEN_TRACK = "Custom Bezier Curve Tween Track";
    
    // public PlayableDirector director;         // 타임라인을 실행 중인 PlayableDirector
    public Transform testplayer;
    public Transform shotTransform;
    
    private PlayableAsset cashTweenClip;
    private string targetTrack;

    public PlayableDirector testDirector;
    public TimelineAsset Target;
    public TimelineAsset NewSourceTimelineAsset;
    public Camera camera;
    
    private void Awake()
    {
        Instance = this;
        camera = Camera.main;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     ChangeTransformTweenLocation(testDirector, testplayer, shotTransform);
        //     // CopyClipsFromTo(NewSourceTimelineAsset, Target);
        // }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // hit.point를 Transform으로 바꾸기 위해 임시 오브젝트 생성
                GameObject tempTarget = new GameObject("TempHitTarget");
                tempTarget.transform.position = hit.point;

                // Transform으로 전달
                ChangeTransformTweenLocation(testDirector, null, tempTarget.transform);
            }
            
            CopyClipsFromTo(NewSourceTimelineAsset, Target);
            ChangeBezierTweenLocation(testDirector, testplayer, shotTransform);
        }
    }

    //클립의 로케이션 변경
    public void ChangeTransformTweenLocation(PlayableDirector director, Transform newStartLocation = null, Transform newEndLocation = null)
    {
        cashTweenClip = FindClipByType<TransformTweenClip>(director);
        
        if (cashTweenClip is TransformTweenClip tweenClip) //형변환
        {
            // 새 Location을 PlayableDirector에 등록
            //start필요없긴하지만 테스트용으로
            if(newStartLocation != null)
                director.SetReferenceValue(tweenClip.startLocation.exposedName, newStartLocation); //동적할당 하기 위해선 exposedName사용
            director.SetReferenceValue(tweenClip.endLocation.exposedName, newEndLocation);
        }
        
        // 변경사항 적용
        director.RebuildGraph();
        director.Play();
    }
    
    public void ChangeBezierTweenLocation(PlayableDirector director, Transform newStartLocation = null, Transform newEndLocation = null)
    {
        cashTweenClip = FindClipByType<CustomBezierCurveTweenTrack>(director) as CustomBezierCurveTweenClip;
        
        if (cashTweenClip is CustomBezierCurveTweenClip tweenClip) //형변환
        {
            // 새 Location을 PlayableDirector에 등록
            if(newStartLocation != null)
                director.SetReferenceValue(tweenClip.startLocation.exposedName, newStartLocation); //동적할당 하기 위해선 exposedName사용
            director.SetReferenceValue(tweenClip.endLocation.exposedName, newEndLocation);
        }
        
        // 변경사항 적용
        director.RebuildGraph();
        director.Play();
    }
    
    private PlayableAsset FindClipByType<T>(PlayableDirector director) //탄환 도착 지점 동적할당
    {
        TimelineAsset timeline = director.playableAsset as TimelineAsset;

        if (!timeline)
        {
            Debug.LogError(" missing timeline asset");
            return null;
        }
        
        foreach (TrackAsset track in timeline.GetOutputTracks()) //트랙 찾기
        {
            // if(track.name != targetTrack) continue; //이름으로 트랙 찾기
            if(track.GetType() != typeof(T)) continue;
            
            foreach (TimelineClip clip in track.GetClips()) //클립 찾기
            {
                return clip.asset as PlayableAsset; //일반화
            }
        }
        
        return null;
    }
    
    
    public void CopyClipsFromTo(TimelineAsset newSource, TimelineAsset target)
    {
        //최적화 필요
        foreach (TrackAsset oldTrack in target.GetOutputTracks())
        {
            foreach (var oldClip in oldTrack.GetClips())
            {
                foreach (var newSourceTrack in newSource.GetOutputTracks())
                {
                    foreach (var newClip in newSourceTrack.GetClips())
                    {
                        if (oldClip.asset.GetType() == newClip.asset.GetType())
                        {
                            Debug.Log("oldClipChange");
                            oldClip.start = newClip.start;
                            oldClip.duration = newClip.duration;
                            oldClip.displayName = newClip.displayName;
                            oldClip.asset = newClip.asset;
                        }
                    }
                }
            }
        }
        
        // foreach (var track in target.GetOutputTracks())
        // {
        //     if (track is GroupTrack groupTrack)
        //     {
        //         Debug.Log("Group: " + groupTrack.name);
        //         foreach (var child in groupTrack.GetChildTracks())
        //         {
        //             Debug.Log($"  └── Child Track: {child.name}");
        //         }
        //     }
        // }
        //
        // foreach (TrackAsset oldTrack in target.GetOutputTracks())
        // {
        //     foreach (var newSourceTrack in newSource.GetOutputTracks())
        //     {
        //         if(oldTrack.GetType() != newSourceTrack.GetType()) continue;
        //
        //         foreach (TimelineClip oldClip in oldTrack.GetClips())
        //         {
        //             TimelineClip newClip = newSourceTrack.GetClips().First();
        //             
        //             oldClip.start = newClip.start;
        //             oldClip.duration = newClip.duration;
        //             oldClip.displayName = newClip.displayName;
        //             oldClip.asset = newClip.asset;
        //         }
        //     }
        // }
        
        // return Task.CompletedTask;
        // testDirector.RebuildGraph();
        //
        // Debug.Log("endLocation SetReferenceValue 적용 완료");
        //
        // testDirector.Play();
    }
}