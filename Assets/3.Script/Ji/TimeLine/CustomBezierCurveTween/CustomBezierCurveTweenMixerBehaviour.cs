using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CustomBezierCurveTweenMixerBehaviour : PlayableBehaviour
{
    bool m_FirstFrameHappened;
    private GameObject trackedTarget;
    private bool isProjectileActive = false;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Transform trackBinding = playerData as Transform;

        if(trackBinding == null)
            return;
        
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
            ScriptPlayable<CustomBezierCurveTweenBehaviour> playableInput = (ScriptPlayable<CustomBezierCurveTweenBehaviour>)playable.GetInput (i);
            CustomBezierCurveTweenBehaviour input = playableInput.GetBehaviour ();

            if(input.endLocation == null)
                continue;

            float inputWeight = playable.GetInputWeight(i);

            if (!m_FirstFrameHappened && !input.startLocation)
            {
                input.startingPosition = defaultPosition;
                input.startingRotation = defaultRotation;
            }
            
            //Harang
            if (!isProjectileActive && inputWeight > 0f)
            {
                isProjectileActive = true;
                trackedTarget.SetActive(true);
                
                if (input.flashParticle) //시작지점에서 Flash 파티클
                {
                    input.flashParticle.transform.position = trackBinding.position; 
                    input.flashParticle.SetActive(true);
                }
            }
            else if (isProjectileActive && inputWeight <= 0f)
            {
                isProjectileActive = false;
                trackedTarget.SetActive(false);
                
                if (input.hitParticle) //끝나는 지점에서 Hit 파티클
                {
                    input.hitParticle.transform.position = trackBinding.position;
                    input.hitParticle.SetActive(true);
                }
            }
            
            float normalisedTime = (float)(playableInput.GetTime() / playableInput.GetDuration ());
            float tweenProgress = input.EvaluateCurrentCurve(normalisedTime);
            
            if (input.tweenPosition)
            {
                positionTotalWeight += inputWeight;

                Vector3 start = input.startingPosition;
                Vector3 end = input.endLocation.position;
                float height = input.arcHeight;

                // 중간 포인트를 기준으로 위로 튀게 만드는 포물선 보간 함수 사용
                Vector3 parabolicPos = GetParabolaPoint(start, end, height, tweenProgress);

                blendedPosition += parabolicPos * inputWeight;
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
    
    Vector3 GetParabolaPoint(Vector3 start, Vector3 end, float height, float t)
    {
        // 중간점을 구한 후 y축으로 올리기
        Vector3 mid = Vector3.Lerp(start, end, t);
        float parabola = 4 * height * t * (1 - t); // 0~1 사이에서 위로 솟는 포물선
        mid.y += parabola;
        return mid;
    }
    
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        m_FirstFrameHappened = false; // 다음에 다시 실행되도록
        isProjectileActive = false;
    }
}
