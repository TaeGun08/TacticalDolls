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
    public ExposedReference<GameObject> flashParticle;
    public ExposedReference<GameObject> hitParticle;
    
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
        clone.flashParticle = flashParticle.Resolve (graph.GetResolver ());
        clone.hitParticle = hitParticle.Resolve (graph.GetResolver ());
        return playable;
    }
}
