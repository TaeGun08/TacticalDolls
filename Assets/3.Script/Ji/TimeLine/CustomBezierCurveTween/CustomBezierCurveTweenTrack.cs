using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(CustomBezierCurveTweenClip))]
[TrackBindingType(typeof(Transform))]
public class CustomBezierCurveTweenTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CustomBezierCurveTweenMixerBehaviour>.Create (graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        var comp = director.GetGenericBinding(this) as Transform;
        if (comp == null)
            return;
        var so = new UnityEditor.SerializedObject(comp);
        var iter = so.GetIterator();
        while (iter.NextVisible(true))
        {
            if (iter.hasVisibleChildren)
                continue;
            driver.AddFromName<Transform>(comp.gameObject, iter.propertyPath);
        }
#endif
        base.GatherProperties(director, driver);
    }
}
