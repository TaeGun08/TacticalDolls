using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class SamplePlayer : MonoBehaviour
{
    public bool isDead = false;
    public bool isCompleteAction = false;
    public Animator animator;
    
    public async Task Excute(int selectedSkill)
    {
        //스킬 애니메이션
        //애니메이션에 맞춰 스킬 효과 적용 (데미지, 힐,)
        Debug.Log($"isCompleteAction {isCompleteAction}");
        
        await Task.Delay(3000);
        Debug.Log($"selectedSkill {selectedSkill} Excuted");
        isCompleteAction = true; //캐릭터 행동 종료
    }
}

public class SamplePlayers : MonoBehaviour
{
    [Header("Bool")]
    [ReadOnly] public bool isDead = false;
    [ReadOnly] public bool isCompleteAction = false;
    private TaskCompletionSource<bool> skillTcs;
    
    public async Task Excute(int selectedSkill)
    {
        SamplePlayer samplePlayer = new SamplePlayer(); //쓰레기값
        SkillSample_Player sampleSkill = new SkillSample_Player(); //쓰레기값
        sampleSkill.MakeSkillSequence(samplePlayer, samplePlayer, sampleAction);
        skillTcs = new TaskCompletionSource<bool>();
        await skillTcs.Task;
    }

    private void sampleAction()
    {
        skillTcs.TrySetResult(true);
        isCompleteAction = true; //캐릭터 행동 종료
    }
}

