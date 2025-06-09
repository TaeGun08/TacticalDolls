using System;
using UnityEngine;

namespace _3.Script.Ji
{
    public class SamplePlayer : MonoBehaviour
    {
        // public void GetTurn(Action turn = null)
        // {
        //     //스킬 사용 대기
        //     turn?.Invoke();
        // }

        public void Excute(Action skillEnd = null)
        {
            //스킬 애니메이션
            //애니메이션에 맞춰 스킬 효과 적용 (데미지, 힐,)
            
            skillEnd?.Invoke();
            //TurnManager.Instance.TurnEndedSource?.TrySetResult(true); //턴 엔드
        }
    }
}
