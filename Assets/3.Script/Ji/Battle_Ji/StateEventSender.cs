using System;
using System.Threading.Tasks;
using UnityEngine;

public class StateEventSender : StateMachineBehaviour
{
    [Header("Animation Event Timing")]
    public string Parameter;

    [Range(0f, 1f)] public float StartNormalizedTime;
    private bool isPassedStart;

    [Range(0f, 1f)] public float EndNormalizedTime;
    private bool isPassedEnd;

    public static event Action<string, GameObject> OnAnimationStartEvent;
    public static event Action<string, GameObject> OnAnimationEndEvent;

    // 상태 진입 시 초기화
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isPassedStart = false;
        isPassedEnd = false;
    }

    // 상태가 업데이트 될 때
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = stateInfo.normalizedTime;

        if (!isPassedStart && currentTime >= StartNormalizedTime)
        {
            OnAnimationStartEvent?.Invoke(Parameter, animator.gameObject);
            isPassedStart = true;
        }

        if (!isPassedEnd && currentTime >= EndNormalizedTime)
        {
            OnAnimationEndEvent?.Invoke(Parameter, animator.gameObject);
            isPassedEnd = true;
        }
    }
}