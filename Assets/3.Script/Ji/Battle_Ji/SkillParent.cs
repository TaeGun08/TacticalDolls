using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum UltType
{
    Normal,
    Ultimate,
    Passive,
}

public interface IUnitSkill
{
    public ActorParent ActorParent { get; }  //스킬 소유자 타입 - Player or Enemy - 컷씬 분류 위해
    public UltType UltType { get; }         //스킬 유형 Nomal, Ultimate, Passive
    public SkillType SkillType { get; }     //스킬 타입 Damage, Heal, Buff
    public RangeType RangeType { get; }     //범위 타입 Straight, Plus, Cross, Around
    
    public string SkillName { get; }        //스킬 이름
    public string SkillInfoText { get; }    //스킬 설명 ( $보간으로 텍스트에 수치를 넣습니다. )
    public class UnitSkillDetails{};        //스킬 수치 정보 (인스펙터용)

    public GameObject Projectile { get; }   //투사체
    public GameObject[] SkillVfXs { get; }  //비주얼이펙트
    public Task SkillExcute();              //적용시킬 스킬 효과
    public Task PlayCutScene();             //적용시킬 스킬 효과
    public void ShotProjectile();           //투사체 발사 메서드
}

public abstract class SkillParent : MonoBehaviour, IUnitSkill
{
    #region Interface
    public abstract ActorParent ActorParent { get; }
    public abstract UltType UltType { get; }
    public abstract SkillType SkillType { get; }
    public abstract RangeType RangeType { get; }

    public abstract string SkillName { get; }
    public abstract string SkillInfoText { get; }
    
    public GameObject Projectile { get; }
    public GameObject[] SkillVfXs { get; }
    
    #endregion
    
    [System.Serializable]
    public class UnitSkillDetails
    {
        public Sprite skillIcon = null;         // 스킬 아이콘
        public int skillValue = 0;              // 데미지, 힐, 보호막 등의 수치
        public int skillRange = 0;              // 공격 범위
        public float bulletSpeed = 0f;          // 발사체 속도
    }
    
    public UnitSkillDetails unitSkillDetails;
    
    protected SamplePlayer CasterCharacter;    //스킬 주인 캐싱
    protected SamplePlayer[] SkillTargets;     //스킬 적용 대상
    protected Vector3 TargetPos;               //목표 지점 (타일)
    protected TaskCompletionSource<bool> SkillEffectTcs; //스킬 종료 타이밍 판단 Tcs
    
    public abstract Task MakeSkillSequence(SamplePlayer samplePlayer, SamplePlayer receiver); //시퀀스 제작
    public abstract SamplePlayer[] CastSkillTarget(); //목표 찾기
    public abstract Task SkillExcute();
    public abstract Task PlayCutScene();
    public abstract void ShotProjectile();  //투사체 발사
}
