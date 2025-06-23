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

public class SkillBase : SkillParent
{
    public override async Task StartSkillAction(List<IDamageAble> targets)
    {
        Debug.Log("StartSkillAction");
        
        if (StartSkillEvents != null)
            foreach (var t in StartSkillEvents)
            {
                t?.Invoke();
            }
        
        // await Task.Delay(100);
    }

    public override Task AffectSkillAction(List<IDamageAble> targets)
    {
        Debug.Log("AffectSkillAction");


        
        for (int i =0; i<RangeSystem.Instance.damageAbles.Count;i++)
        {
            Debug.Log(RangeSystem.Instance.damageAbles[i].GameObject.name);
        }

        int totalAmount = (int)(unitSkillComponents.characterData.Stat.Attack * unitSkillDetails.skillValue); //캐릭터 공격력 * 스킬 배율
        int tickAmount = totalAmount / unitSkillDetails.splitHitCount; //스킬 틱으로 나누기
        
        switch (skillType)
        {
            case SkillType.Damage:
                CombatSystem.Instance.ApplyDamage(unitSkillComponents.characterData, targets, tickAmount);
                break;
            
            case SkillType.Heal:
                CombatSystem.Instance.ApplyHeal(unitSkillComponents.characterData, targets, tickAmount);
                break;
            
            // //ToDo : 
            // case SkillType.Buff:
            //     if (AffectSkillEvents != null)
            //         foreach (var t in AffectSkillEvents)
            //         {
            //             t?.Invoke();
            //         }
            //     break;
        }
        
        return Task.CompletedTask;
    }

    public override async Task EndSkillAction(List<IDamageAble> targets)
    {
        Debug.Log("EndSkillAction");
        
        if (EndSkillEvents != null)
            foreach (var t in EndSkillEvents)
            {
                t?.Invoke();
            }
        
        //await Task.Delay(100);
    }

    #region OldCode
    // private static readonly int ANIMATION_TRIGGER = Animator.StringToHash("ANIMATION_TRIGGER");
    // public override SkillType SkillType => SkillType.Damage;
    // public override RangeType RangeType => RangeType.Single;
    // public override ProjectileType ProjectileType => ProjectileType.FireRate;
    // public override UltType UltType => UltType.Ultimate;
    // private void Start()
    // {
    //     Initialize();
    // }
    //
    // public void Initialize() //초기화
    // {
    //     if (unitSkillDetails.projectile != null) //투사체 비활성화
    //     {
    //         unitSkillDetails.projectile.SetActive(false);
    //     }
    //
    //     if (unitSkillDetails.skillVfX != null)
    //     {
    //         unitSkillDetails.skillVfX.SetActive(false);
    //     }
    //     
    //     SkillEffectTcs =  new TaskCompletionSource<bool>(); //Tcs소스 활성화
    // }

    // public async Task ExcuteSkill(SamplePlayer[] targets, Vector3 targetPosition)
    // {
    //     Debug.Log("ExcuteSkill");
    //     await CharacterSequenceManager.Instance.MakeSequence(unitSkillDetails, targets, targetPosition);
    //     
    //     // SkillEffectTcs.TrySetResult(true); //스킬 적용 종료
    // }

    // public override string SkillName => "샘플스킬 1";
    // public override string SkillInfoText => $"포탄을 발사해 4X4 범위로 {unitSkillDetails.skillValue} 피해를 가합니다.";
    //
    // #region Settings
    // private void Start()
    // {
    //     Initialize();
    // }

    // public void SetCasterCharacter(SamplePlayer samplePlayer)
    // {
    //     CasterCharacter = samplePlayer;
    // }
    // #endregion

    //Animation Sender가 실행시킵니다.
    //애니메이션에서 정의한 타이밍에 호출
    // private void HandleAnimationStart(string param, GameObject characterObject) 
    // {
    //     StateEventSender.OnAnimationStartEvent -= HandleAnimationStart; //해제
    //     
    //     //스킬 호출자와 스킬 보유자가 동일한지 검사
    //     if (characterObject.GetComponent<SamplePlayer>() != CasterCharacter)
    //     {
    //         Debug.LogError("스킬 사용 실패");
    //         SkillEffectTcs.TrySetResult(true); //스킬 적용 종료
    //         return;
    //     }
    //     
    //     _= SkillExcute(); //스킬 발동
    // }

    // public override async Task MakeSkillSequence(SamplePlayer sender, SamplePlayer reciver) //스킬 실행
    // {
    //     Initialize(); //재활용 초기화
    //     
    //     TargetPos = reciver.gameObject.transform.position; //타겟 위치 please fix - 타겟된 타일들의 정보를 받는게 좋음
    //     SkillTargets = CastSkillTarget();
    //     
    //     StateEventSender.OnAnimationStartEvent += HandleAnimationStart;
    //     
    //     // + 컷씬 카메라 연출이랑 동시에 애니메이션 실행
    //     sender.animator.SetTrigger(ANIMATION_TRIGGER);
    //     
    //     await SkillEffectTcs.Task;
    //     
    //     sender.SkillTcs.SetResult(true); //스킬 시퀀스 종료
    // }

    // public override SamplePlayer[] CastSkillTarget()
    // {
    //     //공격 범위 안에 들어온 캐릭터들 체크
    //     return null;
    // }

    // public override Task PlayCutScene()
    // {
    //     throw new NotImplementedException();
    // }

    // public override void ShotProjectile()
    // {
    //     Projectile.transform.DOMove(TargetPos, unitSkillDetails.bulletSpeed)
    //         .SetEase(Ease.Linear)
    //         .OnComplete(async () =>
    //         {
    //             Debug.Log("투사체가 목표 지점에 도달함");
    //             //캐스팅된 적들에게 공격처리
    //             //CombatSystem(this, SkillTargets);
    //                 
    //             //탄환, 폭발, 이펙트 삭제
    //             Projectile.SetActive(false); //투사체 비활성화
    //                 
    //             SkillVfXs[0].SetActive(true); //이펙트 활성화
    //             await Task.Delay(2000);
    //             SkillVfXs[0].SetActive(false); //이펙트 활성화
    //                 
    //             SkillEffectTcs.TrySetResult(true); //스킬 적용 종료
    //         });
    // }

    // public override async Task SkillExcute() //스킬 이벤트 //카메라 무브
    // {
    //     //공격 패턴
    //     //공격 시작
    //     if (Projectile != null) //투사체 보유
    //     {
    //         Projectile.SetActive(true); //투사체 활성화

    //         //카메라 무브
    //     }
    //     else
    //     {
    //         //스킬이 투사체 발사 형식이 아님
    //         
    //     }
    //     
    //     //공격 맞는 시점 - 총알 -> Ray, 투사체 -> DoMove 끝날때 (목표지점에 투사체 도착)
    //     //please fix - CombatSystem.~ 
    //     
    //     SkillEffectTcs.TrySetResult(true); //스킬 적용 종료
    // }
    
    #endregion
    
}
