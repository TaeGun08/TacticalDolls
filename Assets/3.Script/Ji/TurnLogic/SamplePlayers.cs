using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class SamplePlayer
{
    public bool isDead = false;
    public bool isCompleteAction = false;
    
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
    // private SkillData[] skills;
    
    public async Task Excute(int selectedSkill)
    {
        //스킬 애니메이션
        //애니메이션에 맞춰 스킬 효과 적용 (데미지, 힐,)
        
        await Task.Delay(3000);
        
        isCompleteAction = true; //캐릭터 행동 종료
        // return Task.CompletedTask; //끝난 시점에
    }
}

