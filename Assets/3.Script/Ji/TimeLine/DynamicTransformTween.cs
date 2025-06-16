using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

public enum TrackName
{
    Move,
    Fire,
    FireBezier,
}

public class DynamicTransformTweenChanger : MonoBehaviour
{
    public static DynamicTransformTweenChanger Instance;
    
    private const string MOVE_TWEEN_TRACK = "Move Tween Track";
    private const string FIRE_PROJECTILE_TRACK = "Fire Projectile Track";
    private const string CUSTOM_BEZIER_CURVE_TWEEN_TRACK = "Custom Bezier Curve Tween Track";
    
    // public PlayableDirector director;         // 타임라인을 실행 중인 PlayableDirector
    public Transform testplayer;
    public Transform shotTransform;
    
    private CustomBezierCurveTweenClip cashTweenClip;
    private string targetTrack;

    public PlayableDirector testDirector;
    public TimelineAsset Target;
    public TimelineAsset NewSourceTimelineAsset;
    // public TimelineClipModifier modifier;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     ChangeTransformTweenLocation(testDirector, testplayer, shotTransform);
        //     // CopyClipsFromTo(NewSourceTimelineAsset, Target);
        // }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ChangeLocation(testDirector, testTransform);
            CopyClipsFromTo(NewSourceTimelineAsset, Target);
            ChangeBezierTweenLocation(testDirector, testplayer, shotTransform);
        }
    }

    //클립의 로케이션 변경
    // public void ChangeTransformTweenLocation(PlayableDirector director, Transform newStartLocation, Transform newEndLocation)
    // {
    //     cashTweenClip = FindClipByName<TransformTweenClip>(director, TrackName.Move);
    //     
    //     if (cashTweenClip is TransformTweenClip tweenClip) //형변환
    //     {
    //         // 새 Location을 PlayableDirector에 등록
    //         Debug.Log(newStartLocation.gameObject.name);
    //         Debug.Log(newEndLocation.gameObject.name);
    //         director.SetReferenceValue(tweenClip.startLocation.exposedName, newStartLocation); //동적할당 하기 위해선 exposedName사용
    //         director.SetReferenceValue(tweenClip.endLocation.exposedName, newEndLocation);
    //     }
    //     
    //     // 변경사항 적용
    //     director.RebuildGraph();
    //     director.Play();
    // }
    
    public void ChangeBezierTweenLocation(PlayableDirector director, Transform newStartLocation, Transform newEndLocation)
    {
        cashTweenClip = FindClipByName(director, TrackName.FireBezier);
        
        Debug.Log($"tweenClip.startLocation.exposedName : {cashTweenClip.startLocation.exposedName}");
        Debug.Log($"tweenClip.endLocation.exposedName : {cashTweenClip.endLocation.exposedName}");
        // 새 Location을 PlayableDirector에 등록
        director.SetReferenceValue(cashTweenClip.startLocation.exposedName, newStartLocation); //동적할당 하기 위해선 exposedName사용
        director.SetReferenceValue(cashTweenClip.endLocation.exposedName, newEndLocation);
        
        // if (cashTweenClip is CustomBezierCurveTweenClip tweenClip) //형변환
        // {
        //     Debug.Log($"tweenClip.startLocation.exposedName : {tweenClip.startLocation.exposedName}");
        //     Debug.Log($"tweenClip.endLocation.exposedName : {tweenClip.endLocation.exposedName}");
        //     // 새 Location을 PlayableDirector에 등록
        //     director.SetReferenceValue(tweenClip.startLocation.exposedName, newStartLocation); //동적할당 하기 위해선 exposedName사용
        //     director.SetReferenceValue(tweenClip.endLocation.exposedName, newEndLocation);
        // }
        
        // 변경사항 적용
        director.RebuildGraph();
        director.Play();
    }
    
    private CustomBezierCurveTweenClip FindClipByName(PlayableDirector director, TrackName trackName) //탄환 도착 지점 동적할당
    {
        TimelineAsset timeline = director.playableAsset as TimelineAsset;

        if (!timeline)
        {
            Debug.LogError(" missing timeline asset");
            return null;
        }

        Type trackType;
        switch (trackName)
        {
            case TrackName.Move:
                targetTrack = MOVE_TWEEN_TRACK;
                trackType  = typeof(TransformTweenTrack);
                break;
            case TrackName.Fire:
                targetTrack = FIRE_PROJECTILE_TRACK;
                trackType  = typeof(CustomBezierCurveTweenTrack);
                break;
            case TrackName.FireBezier:
                targetTrack = CUSTOM_BEZIER_CURVE_TWEEN_TRACK;
                trackType  = typeof(CustomBezierCurveTweenTrack);
                break;
            default:
                targetTrack = "missing";
                trackType  = typeof(CustomBezierCurveTweenTrack);
                break;
        }

        foreach (TrackAsset track in timeline.GetOutputTracks()) //트랙 찾기
        {
            if(track.name != targetTrack) continue; //이름으로 트랙 찾기
            // if(track.GetType() != typeof(T)) continue;
            
            foreach (TimelineClip clip in track.GetClips()) //클립 찾기
            {
                //TransformTweenClip 타입인 클립만 처리
                if (clip.asset is CustomBezierCurveTweenClip tweenClip)
                {
                    Debug.Log(tweenClip.name);
                    return tweenClip;
                }
            }
        }
        
        return null;
    }
    
    
    public void CopyClipsFromTo(TimelineAsset newSource, TimelineAsset target)
    {
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
        
        // return Task.CompletedTask;
        // testDirector.RebuildGraph();
        //
        // Debug.Log("endLocation SetReferenceValue 적용 완료");
        //
        // testDirector.Play();
    }
}