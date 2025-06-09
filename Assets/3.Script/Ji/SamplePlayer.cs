using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _3.Script.Ji
{
    public class SamplePlayer : MonoBehaviour
    {
        [Header("Bool")]
        [ReadOnly] public bool isDead = false;
        [ReadOnly] public bool isCompleteAction = false;
        
        public Task Excute(int selectedSkill)
        {
            //스킬 애니메이션
            //애니메이션에 맞춰 스킬 효과 적용 (데미지, 힐,)
            
            isCompleteAction = true; //캐릭터 행동 종료
            return Task.CompletedTask; //끝난 시점에
        }
    }
}
