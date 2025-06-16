using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CustomMoveAlongPathMixerBehaviour : PlayableBehaviour
{
    Transform m_TrackBinding;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Transform;

        if (m_TrackBinding == null)
            return;


        int inputCount = playable.GetInputCount ();

        float totalWeight = 0f;
        float greatestWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<CustomMoveAlongPathBehaviour> inputPlayable = (ScriptPlayable<CustomMoveAlongPathBehaviour>)playable.GetInput(i);
            CustomMoveAlongPathBehaviour input = inputPlayable.GetBehaviour ();
            
            totalWeight += inputWeight;

        }
    }
}
