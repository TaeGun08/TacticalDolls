using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MoveParticleTransformClip : PlayableAsset, ITimelineClipAsset
{
    public MoveParticleTransformBehaviour template = new MoveParticleTransformBehaviour ();
    public ExposedReference<Transform> targetLocation;
    public ExposedReference<GameObject> particleObject;
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MoveParticleTransformBehaviour>.Create (graph, template);
        MoveParticleTransformBehaviour clone = playable.GetBehaviour ();
        clone.targetLocation = targetLocation.Resolve (graph.GetResolver ());
        clone.particleObject = particleObject.Resolve(graph.GetResolver());
        return playable;
    }
}

