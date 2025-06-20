using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TransformTweenClip : PlayableAsset, ITimelineClipAsset
{
    public TransformTweenBehaviour template = new TransformTweenBehaviour ();
    public ExposedReference<Transform> startLocation;
    public ExposedReference<Transform> endLocation;
    public ExposedReference<GameObject> hitParticle;
    public ExposedReference<GameObject> flashParticle;
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TransformTweenBehaviour>.Create (graph, template);
        TransformTweenBehaviour clone = playable.GetBehaviour ();
        clone.startLocation = startLocation.Resolve (graph.GetResolver ());
        clone.endLocation = endLocation.Resolve (graph.GetResolver ());
        clone.hitParticle = hitParticle.Resolve (graph.GetResolver ());
        clone.flashParticle = flashParticle.Resolve (graph.GetResolver ());
        return playable;
    }
}