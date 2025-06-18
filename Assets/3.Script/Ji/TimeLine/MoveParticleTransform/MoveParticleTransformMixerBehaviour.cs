using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MoveParticleTransformMixerBehaviour : PlayableBehaviour
{
    bool m_FirstFrameHappened = false;
    public Transform targetLocation;  // 이동 위치
    public GameObject particleObject; // 파티클 프리팹 or 인스턴스
    private bool isParticleActive = false;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Transform trackBinding = playerData as Transform;
        
        if(trackBinding == null)
            return;
        
        int inputCount = playable.GetInputCount();
        
        // 위치 즉시 이동


        // 활성화 및 Play
        // if (!particleObject.activeSelf)
        //     particleObject.SetActive(true);
        
        for (int i = 0; i < inputCount; i++)
        {
            ScriptPlayable<MoveParticleTransformBehaviour> playableInput = (ScriptPlayable<MoveParticleTransformBehaviour>)playable.GetInput (i);
            MoveParticleTransformBehaviour input = playableInput.GetBehaviour();
            
            if(input.targetLocation == null ||  input.particleObject == null)
                continue;
            particleObject = input.particleObject;
            
            float inputWeight = playable.GetInputWeight(i);
            

            //Harang
            if (!isParticleActive && inputWeight > 0f)
            {
                particleObject.transform.position = input.targetLocation.position;
                Debug.Log("Playable inputWeight: " + inputWeight);
                isParticleActive = true;
                particleObject.SetActive(true);
            }
            else if (isParticleActive && inputWeight <= 0f)
            {
                isParticleActive = false;
                particleObject.SetActive(false);
            }
        }
        
        m_FirstFrameHappened = true;
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        m_FirstFrameHappened = false;
        
        //Harang
        if (particleObject != null)
        {
            particleObject.SetActive(false);
        }
    }
    
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        m_FirstFrameHappened = false; // 다음에 다시 실행되도록
    }
}
