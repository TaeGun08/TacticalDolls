using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Exoa.Cameras;
using UnityEngine;
using UnityEngine.Playables;

// public class SkillSequenceInfo
// {
//     public SamplePlayer Sender;
//     public SamplePlayer[] Listeners;
//     public SkillSample SkillSample;
// }

public class CharacterSequenceManager : MonoBehaviour
{
    public static CharacterSequenceManager Instance;
    
    public CameraPerspective touchCamera;

    private void Awake()
    {
        Instance = this;
    }

    public async Task MakeSequence(
        // Vector2Int movePosition,
        SkillSample skillSample,
        SamplePlayer[] listeners,
        Vector3 targetPosition
        )
    {
        
        SkillParent.UnitSkillDetails skillDetails = skillSample.unitSkillDetails;
        
        // 1. 포커스 처리
        CharacterFocus(skillDetails.characterData.transform.position);
        
        // 2. 컷신 재생
        // if (skillDetails.director != null)
        // {
        //     // await PlayCutscene(skillDetails.ultClip.length);
        // }
        
        //동영상으로 교체
        if(skillDetails.ultClip !=null) //ultClip은 애니메이션 클립입니다. 변경하기
            await PlayCutscene(skillDetails.ultClip.length);
        
        Debug.Log("MakeSequence2");
        // 3. 발사체 이동
        await Task.Delay((int)(skillDetails.animationDelay * 1000f));
        
        await ShotProjectile(skillDetails, listeners, targetPosition);
        
        // 시퀀스 완료까지 대기
    }
    
    public async Task PlayCutscene(float ultDuration)
    {
        Debug.Log("PlayCutscene");
        
        //ToDo :: 동영상 실행으로 수정
        await Task.Delay((int)(ultDuration * 1000));
        
        Debug.Log("PlayCutscene");
        // int originalLayer = sender.gameObject.layer;
        //
        // // 컷신 주체만 특정 레이어로
        // sender.gameObject.layer = LayerMask.NameToLayer("CutsceneActor");
        // sender.animator.SetTrigger("Attack"); //타임라인에서 애니메이션 실행 - sender 캐릭터 바인딩 필요
        // // director.Play();
        //
        // await EndCutSceneTcs.Task;
        //
        // // 레이어 복원
        // sender.gameObject.layer = originalLayer;
    }

    private async Task ShotProjectile(SkillParent.UnitSkillDetails skillDetails, SamplePlayer[] targets, Vector3 targetPosition)
    {
        Debug.Log($"touchCamera.transform.position.y {touchCamera.transform.position.y}");
        
        Vector3 midXZ = (skillDetails.characterData.transform.position + targetPosition) / 2f;
        Vector3 focusPoint = new Vector3(midXZ.x, touchCamera.transform.position.y, midXZ.z);
        CharacterFocus(focusPoint); //적과 자신 사이 중간을 포커스
        
        skillDetails.projectile.transform.position = skillDetails.castTransform.position;
        skillDetails.projectile.SetActive(true);
        
        await skillDetails.projectile.transform.DOMove(targets[0].transform.position, skillDetails.projectileSpeed).AsyncWaitForCompletion(); //투사체 발사
        skillDetails.projectile.SetActive(false); //투사체 비활성화
        Debug.Log("탄환 도착");
        
        skillDetails.skillVFX.transform.position = targetPosition; //적 위치로 파티클 이동
        skillDetails.skillVFX.SetActive(true); //이펙트 활성화
        await Task.Delay((int)(skillDetails.vfxDuration * 1000f)); //파티클 지속시간만큼 대기 - 변경하기 ToDo
        skillDetails.skillVFX.SetActive(false); //이펙트 활성화
        
        //ToDo :: 컴뱃시스템 콜
        //foreach
        //await CombatSystem(this, targets);
    }
    
    private void CharacterFocus(Vector3 go)
    {
        Debug.Log("CharacterFocus");
        touchCamera.MoveCameraTo(go);
    }
}
