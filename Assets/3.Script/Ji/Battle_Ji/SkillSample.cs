using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;




//스킬 흐름
//타겟을 설정합니다.
//컷신
//애니메이션 시작
    //공격 실행 //적용 //피격위치로 카메라 이동
//애니메이션 종료
//스킬 종료

public class SkillSample : SkillParent
{
    private static readonly int ANIMATION_TRIGGER = Animator.StringToHash("ANIMATION_TRIGGER");
    public override ActorParent ActorParent => ActorParent.Player;
    public override UltType UltType => UltType.Ultimate;
    public override SkillType SkillType => SkillType.Damage;
    public override RangeType RangeType => RangeType.Straight;
    
    public override string SkillName => "샘플스킬 1";
    public override string SkillInfoText => $"포탄을 발사해 4X4 범위로 {unitSkillDetails.skillValue} 피해를 가합니다.";

    #region Settings
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (Projectile != null) //투사체 비활성화
        {
            Projectile.SetActive(false);
        }

        if (SkillVfXs.Length > 0) //비주얼 이펙트 비활성화
        {
            foreach (var t in SkillVfXs)
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
    #endregion
    
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
        
        _= SkillExcute(); //스킬 발동
    }
        
    public override async Task MakeSkillSequence(SamplePlayer sender, SamplePlayer reciver) //스킬 실행
    {
        Initialize(); //재활용 초기화
        
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

    public override Task PlayCutScene()
    {
        throw new NotImplementedException();
    }

    public override void ShotProjectile()
    {
        Projectile.transform.DOMove(TargetPos, unitSkillDetails.bulletSpeed)
            .SetEase(Ease.Linear)
            .OnComplete(async () =>
            {
                Debug.Log("투사체가 목표 지점에 도달함");
                //캐스팅된 적들에게 공격처리
                //CombatSystem(this, SkillTargets);
                    
                //탄환, 폭발, 이펙트 삭제
                Projectile.SetActive(false); //투사체 비활성화
                    
                SkillVfXs[0].SetActive(true); //이펙트 활성화
                await Task.Delay(2000);
                SkillVfXs[0].SetActive(false); //이펙트 활성화
                    
                SkillEffectTcs.TrySetResult(true); //스킬 적용 종료
            });
    }
    

    public override async Task SkillExcute() //스킬 이벤트 //카메라 무브
    {
        //공격 패턴
        //공격 시작
        if (Projectile != null) //투사체 보유
        {
            Projectile.SetActive(true); //투사체 활성화
            

            
            //카메라 무브
        }
        else
        {
            //스킬이 투사체 발사 형식이 아님
            
        }
        
        //공격 맞는 시점 - 총알 -> Ray, 투사체 -> DoMove 끝날때 (목표지점에 투사체 도착)
        //please fix - CombatSystem.~ 
        
        SkillEffectTcs.TrySetResult(true); //스킬 적용 종료
    }
}
