using System;
using UnityEngine;
using UnityEngine.Playables;

public class TransformTweenMixerBehaviour : PlayableBehaviour
{
    bool m_FirstFrameHappened;
    //Harang
    private GameObject trackedTarget;
    private bool isProjectileActive = false;
    private GameObject cashedParticle;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Transform trackBinding = playerData as Transform;

        if(trackBinding == null)
            return;
        //Harang
        if (trackedTarget == null)
            trackedTarget = trackBinding.gameObject;
        
        Vector3 defaultPosition = trackBinding.position;
        Quaternion defaultRotation = trackBinding.rotation;

        int inputCount = playable.GetInputCount ();
        
        float positionTotalWeight = 0f;
        float rotationTotalWeight = 0f;

        Vector3 blendedPosition = Vector3.zero;
        Quaternion blendedRotation = new Quaternion(0f, 0f, 0f, 0f);

        
        for (int i = 0; i < inputCount; i++)
        {
            ScriptPlayable<TransformTweenBehaviour> playableInput = (ScriptPlayable<TransformTweenBehaviour>)playable.GetInput (i);
            TransformTweenBehaviour input = playableInput.GetBehaviour ();

            if(input.endLocation == null)
                continue;

            if (cashedParticle == null)
            {
                cashedParticle = input.particle;
            }

            
            float inputWeight = playable.GetInputWeight(i);

            if (!m_FirstFrameHappened && !input.startLocation)
            {
                input.startingPosition = defaultPosition;
                input.startingRotation = defaultRotation;
            }
            
            //Harang
            if (!isProjectileActive && inputWeight > 0f) //탄환 생성 안됨 && 클립이 시작됨
            {
                isProjectileActive = true;
                trackedTarget.SetActive(true);
            }
            else if (isProjectileActive && inputWeight <= 0f)
            {
                isProjectileActive = false;
                trackedTarget.SetActive(false);
                
                if (cashedParticle)
                {
                    cashedParticle.transform.position = trackBinding.position;
                    cashedParticle.SetActive(true);
                }
            }
            

            
            float normalisedTime = (float)(playableInput.GetTime() / playableInput.GetDuration ());
            float tweenProgress = input.EvaluateCurrentCurve(normalisedTime);

            if (input.tweenPosition)
            {
                positionTotalWeight += inputWeight;

                blendedPosition += Vector3.Lerp(input.startingPosition, input.endLocation.position, tweenProgress) * inputWeight;
            }

            if (input.tweenRotation)
            {
                rotationTotalWeight += inputWeight;

                Quaternion desiredRotation = Quaternion.Lerp(input.startingRotation, input.endLocation.rotation, tweenProgress);
                desiredRotation = NormalizeQuaternion(desiredRotation);

                if (Quaternion.Dot (blendedRotation, desiredRotation) < 0f)
                {
                    desiredRotation = ScaleQuaternion (desiredRotation, -1f);
                }

                desiredRotation = ScaleQuaternion(desiredRotation, inputWeight);

                blendedRotation = AddQuaternions (blendedRotation, desiredRotation);
            }
        }

        blendedPosition += defaultPosition * (1f - positionTotalWeight);
        Quaternion weightedDefaultRotation = ScaleQuaternion (defaultRotation, 1f - rotationTotalWeight);
        blendedRotation = AddQuaternions (blendedRotation, weightedDefaultRotation);

        trackBinding.position = blendedPosition;
        trackBinding.rotation = blendedRotation;
        
        m_FirstFrameHappened = true;
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        m_FirstFrameHappened = false;
        
        //Harang
        if (trackedTarget != null)
        {
            trackedTarget.SetActive(false);
        }
        
        if (cashedParticle != null)
        {
            cashedParticle.SetActive(false);
        }
    }
    
    static Quaternion AddQuaternions (Quaternion first, Quaternion second)
    {
        first.w += second.w;
        first.x += second.x;
        first.y += second.y;
        first.z += second.z;
        return first;
    }

    static Quaternion ScaleQuaternion (Quaternion rotation, float multiplier)
    {
        rotation.w *= multiplier;
        rotation.x *= multiplier;
        rotation.y *= multiplier;
        rotation.z *= multiplier;
        return rotation;
    }

    static float QuaternionMagnitude (Quaternion rotation)
    {
        return Mathf.Sqrt ((Quaternion.Dot (rotation, rotation)));
    }

    static Quaternion NormalizeQuaternion (Quaternion rotation)
    {
        float magnitude = QuaternionMagnitude (rotation);

        if (magnitude > 0f)
            return ScaleQuaternion (rotation, 1f / magnitude);

        Debug.LogWarning ("Cannot normalize a quaternion with zero magnitude.");
        return Quaternion.identity;
    }
    
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        m_FirstFrameHappened = false; // 다음에 다시 실행되도록
        isProjectileActive = false;
    }
}