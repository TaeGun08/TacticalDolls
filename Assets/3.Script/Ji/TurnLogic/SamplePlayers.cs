using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class SamplePlayer : MonoBehaviour
{
    [Header("Bool")]
    [ReadOnly] public bool isDead = false;
    [ReadOnly] public bool isCompleteAction = false;
    public Animator animator;
    public SkillSample[] SkillSamples;
    public SamplePlayer[] testTargets;
    
    public async Task Excute(int selectedSkill) //스킬 사용
    {
        await CharacterSequenceManager.Instance.MakeSequence(SkillSamples[selectedSkill], testTargets, testTargets[0].transform);
        // await SkillSamples[selectedSkill].ExcuteSkill(testTargets, testTargets[0].transform.position); 태스크 단계 줄임
    }
}

