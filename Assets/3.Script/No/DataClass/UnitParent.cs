using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class UnitParent : MonoBehaviour, IDamageAble
{
    protected static readonly int ATTACKED = Animator.StringToHash("Attacked");
    protected static readonly int DEAD = Animator.StringToHash("Dead");
    
    public abstract IStat Stat { get; }
    public abstract Collider MainCollider { get; }
    public abstract GameObject GameObject { get; }
    public abstract int Team { get; }
    public abstract SkillParent[] HasSkills { get; set; }
    public abstract Action OnHpChanged { get; set; }
    
    public abstract Animator Animator { get; }

    public Transform hpBarTransform;
    public abstract string PrefabName { get; set; }

    [SerializeField] protected Sprite characterIcon;
    public Sprite CharacterIcon => characterIcon;

    public abstract Task Excute(int selectedSkill, List<IDamageAble> targets, Transform targetPoint);
    
    public abstract void TakeDamage(CombatEvent combatEvent);

    public abstract void TakeHeal(HealEvent combatEvent);

    public abstract void TakeBuff(BuffEvent combatEvent);
    
    protected IEnumerator DeadCorotuine()
    {
        Animator.SetTrigger(DEAD);
        Stat.IsDead = true;
        Tile usingTile = TileManager.Instance.GetCurrentTileByIDamageAble(this);
        usingTile.isUsingTile = false;
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
