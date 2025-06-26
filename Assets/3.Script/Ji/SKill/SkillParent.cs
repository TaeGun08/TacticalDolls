using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Febucci.UI.Core;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

// public enum UltType
// {
//     Normal,
//     Ultimate,
//     Passive,
// }

// public enum ProjectileFireType
// {
//     None,
//     Once, 
//     FireRate,
// }

public enum ProjectilePathType
{
    None,
    Straight,     // 직선
    // Parabolic,    // 곡사 (포물선)
    Homing,       // 추적 (적을 따라감) //Move Tween Blend Curves 값 조정으로 구현하기
    Spread,       // 확산 (샷건 스타일)
    Curved,       // 일반적인 곡선 (베지어 등)
    // Zigzag,       // 지그재그
    // Spiral,       // 나선형
    // Wave,         // 물결처럼 흔들림
    // Bounce        // 튕김 (벽 반사)
}

public enum TargetType
{
    Tile,
    Ally,
    Enemy,
}

// public ActorParent ActorParent { get; }  //스킬 소유자 타입 - Player or Enemy - 컷씬 분류 위해
// public UltType UltType { get; }         //스킬 유형 Nomal, Ultimate, Passive
// public interface IUnitSkill
// {
//     public SkillType SkillType { get; }     //스킬 타입 Damage, Heal, Buff
//     public RangeType RangeType { get; }     //범위 타입 Straight, Plus, Cross, Around
//     public ProjectileType ProjectileType { get; }
//     public class UnitSkillDetails{};        //스킬 수치 정보 (인스펙터용)
// }
// [LabelText("발사 형식 타입")] [SerializeField] private ProjectileFireType projectileType; //미사용

public abstract class SkillParent : MonoBehaviour //, IUnitSkill
{
    [Header("Enum")] [Space]
    [LabelText("스킬 기본 타입")] public SkillType skillType; //미사용
    [LabelText("스킬이 타겟할 수 있는 유형 타입")] public TargetType targetType; //미사용
    [LabelText("스킬 범위 타입")] public RangeType rangeType; //사용중
    [LabelText("발사체 궤적 타입")] public ProjectilePathType projectilePathType; //사용중

    [Space] [Header("String")] [Space]
    public string skillName;
    
    [Space]
    [TextArea(3, 10)] // 최소 3줄, 최대 10줄
    public string skillInfoText;

    
    [System.Serializable]
    public class UnitSkillDetails
    {
        [Space] [Header("값")] [Space]
        
        [LabelText("스킬 ID")]
        [Tooltip("스킬을 식별하기 위한 ID값 입니다.")] public string skillID;
        
        [LabelText("스킬 효과 값")]
        [Tooltip("데미지, 힐, 보호막 등이 적용될 수치입니다.")] [Range(0f, 5.0f)]public float skillValue = 0;
        
        [LabelText("공격 범위")] 
        [Tooltip("스킬이 타격할 수 있는 범위 값 입니다.")] public int areaOfEffect = 0;
        
        [LabelText("타격 횟수")] 
        [Tooltip("총알이 몇 번 발사되어 타격하는지를 의미합니다.")][Range(1,50)] public int splitHitCount = 1;
    }

    [System.Serializable]
    public class UnitSkillComponents
    {
        [Space] [Header("컴포넌트")] [Space]
        
        //발동시킬 버프 추가?
        
        [LabelText("스킬 보유 캐릭터")] [Required]
        [Tooltip("스킬을 가진 캐릭터입니다.")]  public UnitParent characterData;
        
        [LabelText("스킬 아이콘")] [Required]
        [Tooltip("스킬 아이콘입니다.")] public Sprite skillIconSprite;
        
        [LabelText("타임라인 디렉터")] [Required]
        [Tooltip("스킬 타임라인을 실행하는  디렉터입니다.")] public PlayableDirector director;
        
        [LabelText("필살기 비디오클립")] 
        [Tooltip("필살기 동영상 클립 입니다.")] public VideoClip ultClip; //ToDo:타임라인으로 변경하기
    }
    
    public UnitSkillDetails unitSkillDetails;
    public UnitSkillComponents unitSkillComponents;


    //ToDo 리스트로 바꾸고 초기화
    public Action[] StartSkillEvents = new Action[10];
    public Action[] AffectSkillEvents = new Action[10];
    public Action[] EndSkillEvents = new Action[10];
    
    public abstract Task StartSkillAction(List<IDamageAble> targets);
    public abstract Task AffectSkillAction(List<IDamageAble> targets);
    public abstract Task EndSkillAction(List<IDamageAble> targets);
    
    private void Start()
    {
        if(CharacterSequenceManager.Instance != null)
            BindSignalReceiver();
    }
    
    void BindSignalReceiver() //시그널 바인딩
    {
        TimelineAsset timeline = unitSkillComponents.director.playableAsset as TimelineAsset;
        
        if (timeline == null)
        {
            Debug.LogError("PlayableAsset is not a TimelineAsset");
            return;
        }
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is SignalTrack signalTrack)
            {
                unitSkillComponents.director.SetGenericBinding(signalTrack, CharacterSequenceManager.Instance.signalReceiver);
                Debug.Log("SignalTrack 바인딩 완료");
            }
        }
    }
}
