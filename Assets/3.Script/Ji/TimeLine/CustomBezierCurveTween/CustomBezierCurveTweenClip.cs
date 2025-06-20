using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CustomBezierCurveTweenClip : PlayableAsset, ITimelineClipAsset
{
    public CustomBezierCurveTweenBehaviour template = new CustomBezierCurveTweenBehaviour ();
    public ExposedReference<Transform> startLocation;
    public ExposedReference<Transform> endLocation;
    public ExposedReference<GameObject> particle;
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CustomBezierCurveTweenBehaviour>.Create (graph, template);
        CustomBezierCurveTweenBehaviour clone = playable.GetBehaviour ();
        clone.startLocation = startLocation.Resolve (graph.GetResolver ());
        clone.endLocation = endLocation.Resolve (graph.GetResolver ());
        clone.particle = particle.Resolve (graph.GetResolver ());
        return playable;
    }
}
