using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

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
    public class UnitSkillDetails{};        //스킬 수치 정보 (인스펙터용)
}

public abstract class SkillParent : MonoBehaviour, IUnitSkill
{
    #region Interface
    public abstract ActorParent ActorParent { get; }
    public abstract UltType UltType { get; }
    public abstract SkillType SkillType { get; }
    public abstract RangeType RangeType { get; }

    public string skillName;
    
    [TextArea(3, 10)] // 최소 3줄, 최대 10줄
    public string skillInfoText; // 소문자 필드로 변경
    #endregion
    
    
    [System.Serializable]
    public class UnitSkillDetails
    {
        public int skillValue = 0;              // 데미지, 힐, 보호막 등의 수치
        public int skillRange = 0;              // 공격 사거리
        public int areaOfEffect = 0;            // 공격 범위
        public float projectileSpeed = 0f;      // 발사체 속도
        public float animationDelay = 0f;       // 애니메이션 - 효과 대기시간
        public float vfxDuration = 0f;          // 파티클 지속시간

        public AnimationClip ultClip;
        public SamplePlayer characterData;
        public Transform castTransform;
        public Sprite skillIconSprite;
        public GameObject projectile;
        public GameObject skillVFX;
        public PlayableDirector director;
        public string skillName; //테스트용
    }
    
    public UnitSkillDetails unitSkillDetails;
    protected TaskCompletionSource<bool> SkillEffectTcs; //스킬 종료 타이밍 판단 Tcs
}
