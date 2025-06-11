using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

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
        public Sprite skillIcon;            // 스킬 아이콘
        public int skillDamage = 0;         // 데미지
        public int skillRange = 0;          // 공격 범위
        public float skillDelay = 1.0f;     // 공격 타이밍 지연 //또는 애니메이션 이벤트로 관리 
    }
    public UnitSkillDetails unitSkillDetails;
    
    protected GameObject[] SkillEffectPrefabs; //스킬 VFX
    protected SamplePlayer SamplePlayer;       //스킬 주인 캐싱
    protected Sequence SkillSequence;          //시퀀스 캐싱
    protected Vector3 TargetPos;               //목표 지점 (타일)
    protected GameObject[] SkillVFX;           //스킬 비주얼 효과
    
    public abstract Task MakeSkillSequence(SamplePlayer samplePlayer, SamplePlayer reciver); //시퀀스 제작
    public abstract Task SkillEffect();  // 적용시킬 스킬 효과
}

public class SkillSample_Player : PlayerSkillParent
{
    public override UltType UltType => UltType.Ultimate;
    public override SkillType SkillType => SkillType.Damage;
    public override RangeType RangeType => RangeType.Straight;

    public override string SkillName => "샘플스킬 1";
    public override string SkillInfoText => $"포탄을 발사해 4X4 범위로 {10} 피해를 가합니다.";
    
    
    public override async Task MakeSkillSequence(SamplePlayer sender, SamplePlayer reciver) //스킬 실행
    {
        TargetPos = reciver.gameObject.transform.position; //타겟 위치 please fix - 타겟된 타일들의 정보를 받는게 좋음
        
        SkillSequence = DOTween.Sequence();
        
        SkillSequence.AppendCallback(()=>
        {
            // + 카메라 연출이랑 동시에
            sender.animator.SetTrigger("animationTrigger");
        });

        SkillSequence.Pause(); //대기
        
        await StateEventSender.WaitForAnimationEvent(sender.gameObject, "AttackImpact");
        SkillEffect(); //스킬 이벤트 발생 (실제로 적용되는 전투 효과 (데미지, 힐, 보호막))
        
        SkillSequence.Play(); //시작
        
        // SkillSequence.AppendInterval(unitSkillDetails.skillDelay); //일정 시간 딜레이
        
        // SkillSequence.AppendCallback(() =>
        // {
        //     //스킬 이벤트 발생 (실제로 적용되는 전투 효과 (데미지, 힐, 보호막))
        //     SkillEffect();
        // });
        
        SkillSequence.OnComplete(() =>
        {
            sender.SkillTcs.SetResult(true); //시퀀스 종료
        });
    }

    public override async Task SkillEffect() //스킬 이벤트 //카메라 무브
    {
        //공격 맞는 시점 - 총알 -> Ray, 투사체 -> DoMove 끝날때 (목표지점에 투사체 도착)
        //please fix - CombatSystem.~ 
    }
}
