using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CustomMoveAlongPathClip : PlayableAsset, ITimelineClipAsset
{
    public CustomMoveAlongPathBehaviour template = new CustomMoveAlongPathBehaviour();
    
    public override double duration => template.GetEstimatedDuration();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CustomMoveAlongPathBehaviour>.Create(graph, template);
        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.Looping | ClipCaps.Blending;
}
