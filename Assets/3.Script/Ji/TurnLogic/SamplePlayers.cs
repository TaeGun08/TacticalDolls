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
    public TaskCompletionSource<bool> SkillTcs;
    
    public async Task Excute(int selectedSkill)
    {
        //스킬 애니메이션
        //애니메이션에 맞춰 스킬 효과 적용 (데미지, 힐,)
        SamplePlayer samplePlayer = new SamplePlayer(); //쓰레기값
        SkillSample_Player sampleSkill = new SkillSample_Player(); //쓰레기값
        sampleSkill.MakeSkillSequence(samplePlayer, samplePlayer);
        SkillTcs = new TaskCompletionSource<bool>();
        await SkillTcs.Task;
    }
}

// public class SamplePlayers : MonoBehaviour
// {
//     [Header("Bool")]
//     [ReadOnly] public bool isDead = false;
//     [ReadOnly] public bool isCompleteAction = false;
//     private TaskCompletionSource<bool> skillTcs;
//     
//     public async Task Excute(int selectedSkill)
//     {
//         SamplePlayer samplePlayer = new SamplePlayer(); //쓰레기값
//         SkillSample_Player sampleSkill = new SkillSample_Player(); //쓰레기값
//         sampleSkill.MakeSkillSequence(samplePlayer, samplePlayer);
//         skillTcs = new TaskCompletionSource<bool>();
//         await skillTcs.Task;
//     }
//
//     private void sampleAction()
//     {
//         skillTcs.TrySetResult(true);
//         isCompleteAction = true; //캐릭터 행동 종료
//     }
// }

