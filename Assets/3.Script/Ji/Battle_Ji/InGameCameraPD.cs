using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class InGameCameraPD : MonoBehaviour
{
    [LabelText("Cameras")]
    [SerializeField] private CinemachineVirtualCamera topViewCamera;
    [SerializeField] private CinemachineVirtualCamera characterMiddleZoomCamera;
    
    [LabelText("TimeLine")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TimelineAsset timelineAsset;
    
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip[] animationClip;
    [SerializeField] private GameObject effect;
    
    [ReadOnly] private bool isPlaying = false;
    
    void Start()
    {
        director.Play(); // Timeline 재생
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlaySkill(animator,animationClip[0], effect, timelineAsset);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlaySkill(animator,animationClip[1], effect, timelineAsset);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlaySkill(animator,animationClip[2], effect, timelineAsset);
        }
        // if (director.state == PlayState.Playing && isPlaying == false)
        // {
        //     isPlaying = true;
        //     if (director.time >= director.duration)
        //     {
        //         Debug.Log("Timeline time reached end.");
        //         // 여기에 종료 후 작업 실행
        //         
        //         isPlaying = false;
        //     }
        // }
        // else if (isPlaying)
        // {
        //     Debug.Log("Timeline stopped (early or finished)");
        //     isPlaying = false;
        // }
    }
    
    public void PlaySkillTimeline(Animator caster, TimelineAsset timelineAsset)
    {
        director.playableAsset = timelineAsset;

        // 트랙 찾기 + 바인딩 (예: 애니메이션 트랙)
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            if (track is AnimationTrack)
            {
                director.SetGenericBinding(track, caster);
            }
            else if (track.name.Contains("Effect"))
            {
                // 이펙트용 트랙도 따로 바인딩 가능
            }
        }

        director.Play();
    }
    
    [CreateAssetMenu(menuName = "Skill/SkillDefinition")]
    public class SkillDefinition : ScriptableObject
    {
        public string skillName;
        public TimelineAsset timeline;
        public GameObject projectilePrefab;
        public float damage;
        
        
    }
    
    public void PlaySkill(Animator caster, AnimationClip animClip, GameObject effect, TimelineAsset skillTimeLine)
    {
        // 1. 타임라인 설정
        director.playableAsset = skillTimeLine;
        
        // 2. 애니메이션 트랙에 바인딩
        foreach (var track in skillTimeLine.GetOutputTracks())
        {
            if (track is AnimationTrack animTrack)
            {
                director.SetGenericBinding(animTrack, caster);
                // 3. 클립 교체 (중요!)
                foreach (var clip in animTrack.GetClips())
                {
                    AnimationPlayableAsset animPlayableAsset = clip.asset as AnimationPlayableAsset;
                    if (animPlayableAsset != null)
                    {
                        animPlayableAsset.clip = animClip; // 여기에 스킬별 애니메이션 클립 넣기!
                        
                        clip.start = Random.Range(0.0f, 0.5f);              // Timeline 상 시작 시간 (초)
                        clip.duration = Random.Range(0.7f, 2.5f);          // 클립 재생 시간
                        clip.clipIn = Random.Range(0.0f, 2.5f);          // 애니메이션의 시작 지점 (클립 내부 offset)
                    }
                }
            }
        }
        
        director.time = 0.0;
        director.Play();
    }
}
