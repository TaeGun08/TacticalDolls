using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Exoa.Cameras;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;
using Object = System.Object;

public class CharacterSequenceManager : MonoBehaviour
{
    public static CharacterSequenceManager Instance;
    public SignalReceiver signalReceiver;
    
    [SerializeField] private CameraPerspective touchCamera;
    private SkillBase cashedSkill;
    private SkillParent.UnitSkillDetails cashedSkillDetails;
    private SkillParent.UnitSkillComponents cashedSkillComponents;
    private List<IDamageAble> cashedSkillTargets;
    private Quaternion originalRotation;
    private Vector3 direction;
    
    private void Awake()
    {
        Instance = this;
    }

    public async Task MakeSequence(
        SkillBase skillSample,
        List<IDamageAble> listeners,
        Transform targetPosition
        )
    {
        
        cashedSkill = skillSample;
        cashedSkillDetails = skillSample.unitSkillDetails;
        cashedSkillComponents =  skillSample.unitSkillComponents;
        cashedSkillTargets =  listeners;
        
        await cashedSkill.StartSkillAction(listeners);
        
        // 1. 포커스 처리
        // CharacterFocus(cashedSkillComponents.characterData.transform.position);
        touchCamera.MoveCameraTo(cashedSkillComponents.characterData.transform.position);
            
        originalRotation = cashedSkillComponents.characterData.gameObject.transform.rotation; //원래 회전값 저장
        
        direction = (targetPosition.position - cashedSkillComponents.characterData.gameObject.transform.position).normalized;
        direction.y = 0f; // Y축은 무시


        await cashedSkillComponents.characterData.gameObject.transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.5f) //적 방향으로 회전
            .SetEase(Ease.OutSine)
            .AsyncWaitForCompletion();
        
        // 2. 컷신 재생 -> 폐기
        //동영상으로 교체
        // if (cashedSkillComponents.ultClip != null) //ultClip은 애니메이션 클립입니다. 변경하기
        //     await PlayCutscene(cashedSkillComponents.ultClip);
        
        
        // 3. 애니메이션 & 탄환 발사 타임라인 재생
        if (cashedSkillComponents.director != null)
        {
            if (cashedSkill.projectilePathType == ProjectilePathType.Straight)
            {
                await ChangeTweenLocationByType<TransformTweenTrack>(cashedSkillComponents.director, targetPosition);
            }
            else if (cashedSkill.projectilePathType == ProjectilePathType.Curved)
            {
                await ChangeTweenLocationByType<CustomBezierCurveTweenTrack>(cashedSkillComponents.director, targetPosition);
            }
            
            await AwaitTimelineEnd(cashedSkillComponents.director);
        }
        
        touchCamera.MoveCameraTo(cashedSkillComponents.characterData.transform.position);
        
        //적과 자신 사이 중간을 포커스
        // {   
        //     // Debug.Log($"touchCamera.transform.position.y {touchCamera.transform.position.y}");
        //     Vector3 midXZ = (cashedSkillComponents.characterData.transform.position + targetPosition.position) / 2f;
        //     Vector3 focusPoint = new Vector3(midXZ.x, touchCamera.transform.position.y, midXZ.z);
        //     CharacterFocus(focusPoint); 
        // }
        

        await cashedSkill.EndSkillAction(listeners);
        
        await cashedSkillComponents.characterData.gameObject.transform.DORotateQuaternion(originalRotation, 0.5f).SetEase(Ease.InSine).AsyncWaitForCompletion(); //원래 회전값으로 복귀

        // 시퀀스 완료까지 대기
    }
    
    //타임라인이 끝날 때까지 기다립니다.
    private Task AwaitTimelineEnd(PlayableDirector director)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        void OnStopped(PlayableDirector d)
        {
            director.stopped -= OnStopped;
            tcs.TrySetResult(true);
        }

        director.stopped += OnStopped;
        director.Play(); // 타임라인 재생

        return tcs.Task; // 끝날 때까지 await 대기
    }
    
    //탄환 도착 지점 동적할당
    private Task ChangeTweenLocationByType<T>(PlayableDirector director, Transform newEndLocation = null, Transform newStartLocation = null) where T : class //명시적 클래스 보장
    {
        // Debug.Log($"ChangeTweenLocationByType {newEndLocation.gameObject.name}");
        TimelineAsset timeline = director.playableAsset as TimelineAsset;

        if (!timeline)
        {
            Debug.LogError(" missing timeline asset");
            return Task.CompletedTask;
        }
        
        foreach (TrackAsset track in timeline.GetOutputTracks()) //트랙 찾기
        {
            // if(track.name != targetTrack) continue; //이름으로 트랙 찾기
            if(track.GetType() != typeof(T)) continue; //타입으로 트랙 찾기
            
            var targetClip = track.GetClips().First().asset;
            
            if (targetClip is TransformTweenClip tweenClip) //형변환
            {
                // 새 Location을 PlayableDirector에 등록
                if(newStartLocation != null)
                    director.SetReferenceValue(tweenClip.startLocation.exposedName, newStartLocation); 
                director.SetReferenceValue(tweenClip.endLocation.exposedName, newEndLocation); //동적할당 하기 위해선 exposedName사용
            }
            else if (targetClip is CustomBezierCurveTweenClip bezierCurveTweenClip) //형변환
            {
                if(newStartLocation != null)
                    director.SetReferenceValue(bezierCurveTweenClip.startLocation.exposedName, newStartLocation); 
                director.SetReferenceValue(bezierCurveTweenClip.endLocation.exposedName, newEndLocation);
            }
        }
        
        cashedSkillComponents.director.RebuildGraph(); //타임라인 재구성
        
        return Task.CompletedTask;
    }
    
    // public async Task PlayCutscene(VideoClip videoClip)
    // {
    //
    //     await Task.Delay((int)((videoClip ? videoClip.length : 1f) * 1000)); //비디오 시간만큼 대기
    // }
    
    // private void CharacterFocus(Vector3 go)
    // {
    //     touchCamera.MoveCameraTo(go);
    // }

    public void ProjectileSignalListener() //발사체가 맞을 경우 수치를 틱으로 나눠 적용합니다.
    {
        Debug.Log("ProjectileSignalListener");
        cashedSkill.AffectSkillAction(cashedSkillTargets);
    }
    
    // public void TouchCameraSignalListener()
    // {
    //     Debug.Log("TouchCameraSignalListener");
    //     touchCamera.MoveCameraTo(cashedSkillComponents.characterData.gameObject.transform.position);
    // }
}
