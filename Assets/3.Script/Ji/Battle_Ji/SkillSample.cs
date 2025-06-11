using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public enum UltType
{
    Normal,
    Ultimate,
    Passive,
}

public interface IUnitSkill
{
    public ActorParent ActorParent { get;}  //스킬 소유자 타입 - Player or Enemy - 컷씬 분류 위해
    public UltType UltType { get; }         //스킬 유형 Nomal, Ultimate, Passive
    public SkillType SkillType { get; }     //스킬 타입 Damage, Heal, Buff
    public RangeType RangeType { get; }     //범위 타입 Straight, Plus, Cross, Around
    
    public string SkillName { get; }        //스킬 이름
    public string SkillInfoText { get; }    //스킬 설명 ( $보간으로 텍스트에 수치를 넣습니다. )
}

public abstract class PlayerSkillParent : MonoBehaviour, IUnitSkill //플레이어 전용 스킬 부모
{
    #region Interface
    public ActorParent ActorParent => ActorParent.Player;
    public abstract UltType UltType { get; }
    public abstract SkillType SkillType { get; }
    public abstract RangeType RangeType { get; } //please fix - None이 추가되야 합니다.

    public abstract string SkillName { get; }
    public abstract string SkillInfoText { get; }
    
    #endregion
    
    [System.Serializable]
    public class UnitSkillDetails
    {
        public Sprite skillIcon;                // 스킬 아이콘
        public int skillValue = 0;              // 데미지, 힐, 보호막 등의 수치
        public int skillRange = 0;              // 공격 범위
        public float bulletSpeed;               //발사체 속도
    }
    
    public UnitSkillDetails unitSkillDetails;
    
    public GameObject bulletPrefab;            //발사체
    public GameObject[] skillVFXPrefabs;       //스킬 비주얼 효과
    
    protected SamplePlayer CasterCharacter;    //스킬 주인 캐싱
    protected SamplePlayer[] SkillTargets;     //스킬 적용 대상
    protected Vector3 TargetPos;               //목표 지점 (타일)
    protected TaskCompletionSource<bool> SkillEffectTcs; //스킬 종료 타이밍 판단 Tcs
    
    public abstract Task MakeSkillSequence(SamplePlayer samplePlayer, SamplePlayer receiver); //시퀀스 제작
    public abstract SamplePlayer[] CastSkillTarget();
    public abstract Task SkillEffect();  // 적용시킬 스킬 효과
}

public class SkillSample_Player : PlayerSkillParent
{
    private static readonly int ANIMATION_TRIGGER = Animator.StringToHash("animationTrigger");
    public override UltType UltType => UltType.Ultimate;
    public override SkillType SkillType => SkillType.Damage;
    public override RangeType RangeType => RangeType.Straight;
    
    public override string SkillName => "샘플스킬 1";
    public override string SkillInfoText => $"포탄을 발사해 4X4 범위로 {unitSkillDetails.skillValue} 피해를 가합니다.";

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (bulletPrefab != null) //투사체 비활성화
        {
            bulletPrefab.SetActive(false);
        }

        if (skillVFXPrefabs.Length > 0) //비주얼 이펙트 비활성화
        {
            foreach (var t in skillVFXPrefabs)
            {
                t.gameObject.SetActive(false);
            }
        }
        
        SkillEffectTcs =  new TaskCompletionSource<bool>(); //Tcs소스 활성화
    }

    public void SetCasterCharacter(SamplePlayer samplePlayer)
    {
        CasterCharacter = samplePlayer;
    }
    
    //Animation Sender가 실행시킵니다.
    //애니메이션에서 정의한 타이밍에 호출
    private void HandleAnimationStart(string param, GameObject characterObject) 
    {
        StateEventSender.OnAnimationStartEvent -= HandleAnimationStart; //해제
        
        //스킬 호출자와 스킬 보유자가 동일한지 검사
        if (characterObject.GetComponent<SamplePlayer>() != CasterCharacter)
        {
            Debug.LogError("스킬 사용 실패");
            SkillEffectTcs.TrySetResult(true); //스킬 적용 종료
            return;
        } 
        
        _= SkillEffect(); //스킬 발동
    }
        
    public override async Task MakeSkillSequence(SamplePlayer sender, SamplePlayer reciver) //스킬 실행
    {
        TargetPos = reciver.gameObject.transform.position; //타겟 위치 please fix - 타겟된 타일들의 정보를 받는게 좋음
        SkillTargets = CastSkillTarget();
        
        StateEventSender.OnAnimationStartEvent += HandleAnimationStart;
        
        // + 컷씬 카메라 연출이랑 동시에 애니메이션 실행
        sender.animator.SetTrigger(ANIMATION_TRIGGER);
        
        await SkillEffectTcs.Task;
        
        sender.SkillTcs.SetResult(true); //스킬 시퀀스 종료
    }

    public override SamplePlayer[] CastSkillTarget()
    {
        //공격 범위 안에 들어온 캐릭터들 체크
        return null;
    }

    public override async Task SkillEffect() //스킬 이벤트 //카메라 무브
    {
        //공격 패턴
        //공격 시작
        if (bulletPrefab != null) //투사체 보유
        {
            bulletPrefab.SetActive(true);
            bulletPrefab.transform.DOMove(TargetPos, unitSkillDetails.bulletSpeed)        
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Debug.Log("투사체가 목표 지점에 도달함");
                    //캐스팅된 적들에게 공격처리
                    //CombatSystem(castCharacter);
                    // 후처리: 예) 폭발, 이펙트, 삭제 등
                    bulletPrefab.SetActive(false); //투사체 비활성화
                    skillVFXPrefabs[0].SetActive(true);
                    
                    SkillEffectTcs.TrySetResult(true); //스킬 적용 종료
                });
        }

        //공격 맞는 시점 - 총알 -> Ray, 투사체 -> DoMove 끝날때 (목표지점에 투사체 도착)
        //please fix - CombatSystem.~ 
        
        SkillEffectTcs.TrySetResult(true); //스킬 적용 종료
    }
}
