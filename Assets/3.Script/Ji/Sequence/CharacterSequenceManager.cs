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

public class CharacterSequenceManager : MonoBehaviour
{
    public static CharacterSequenceManager Instance;
    
    // private static readonly int ANIMATION_TRIGGER1 = Animator.StringToHash("Skill_1");
    // private static readonly int ANIMATION_TRIGGER2 = Animator.StringToHash("Skill_2");
    // private static readonly int ANIMATION_TRIGGER3 = Animator.StringToHash("Skill_3");
    
    [SerializeField] private CameraPerspective touchCamera;
    private SkillBase cashedSkill;
    private SkillParent.UnitSkillDetails cashedSkillDetails;
    private SkillParent.UnitSkillComponents cashedSkillComponents;
    
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
        
        // 1. 포커스 처리
        CharacterFocus(cashedSkillComponents.characterData.transform.position);
        
        // 2. 컷신 재생
        //동영상으로 교체
        if (cashedSkillComponents.ultClip != null) //ultClip은 애니메이션 클립입니다. 변경하기
            await PlayCutscene(cashedSkillComponents.ultClip);
        
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
        
            cashedSkillComponents.director.Play();
        }
        
        //적과 자신 사이 중간을 포커스
        {   
            // Debug.Log($"touchCamera.transform.position.y {touchCamera.transform.position.y}");
            Vector3 midXZ = (cashedSkillComponents.characterData.transform.position + targetPosition.position) / 2f;
            Vector3 focusPoint = new Vector3(midXZ.x, touchCamera.transform.position.y, midXZ.z);
            CharacterFocus(focusPoint); 
        }
        
        // 시퀀스 완료까지 대기
    }
    
    //탄환 도착 지점 동적할당
    private Task ChangeTweenLocationByType<T>(PlayableDirector director, Transform newEndLocation = null, Transform newStartLocation = null) where T : class //명시적 클래스 보장
    {
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
            
            T targetClip = track.GetClips().First().asset as T;
            
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
    
    public async Task PlayCutscene(VideoClip videoClip)
    {
        Debug.Log("PlayCutscene");
        
        //ToDo :: 동영상 실행으로 수정
        await Task.Delay((int)((videoClip ? videoClip.length : 1f) * 1000)); //비디오 시간만큼 대기
    }
    
    private void CharacterFocus(Vector3 go)
    {
        Debug.Log("CharacterFocus");
        touchCamera.MoveCameraTo(go);
    }

    public void ProjectileSignalListener() //발사체가 맞을 경우 수치를 틱으로 나눠 적용합니다.
    {
        //Debug.Log($"{cashedSkillDetails.skillValue} / {cashedSkillDetails.splitHitCount} = {cashedSkillDetails.skillValue / cashedSkillDetails.splitHitCount}");
        
        //ToDo 캐릭터 스킬 데미지 (캐릭터 공격력) 을 적용해야 합니다.
        //cashedSkillDetails.skillValue / skillValuecashedSkillDetails.splitHitCount //틱 데미지
    }
}
