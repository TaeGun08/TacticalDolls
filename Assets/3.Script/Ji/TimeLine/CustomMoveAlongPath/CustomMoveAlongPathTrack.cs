using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(CustomMoveAlongPathClip))]
[TrackBindingType(typeof(CustomMoveAlongPathClip))]
public class CustomMoveAlongPathTrack : TrackAsset
{
//     public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
//     {
//         return ScriptPlayable<CustomMoveAlongPathMixerBehaviour>.Create (graph, inputCount);
//     }
//
//     // Please note this assumes only one component of type Transform on the same gameobject.
//     public override void GatherProperties (PlayableDirector director, IPropertyCollector driver)
//     {
// #if UNITY_EDITOR
//         Transform trackBinding = director.GetGenericBinding(this) as Transform;
//         if (trackBinding == null)
//             return;
//
// #endif
//         base.GatherProperties (director, driver);
//     }
}
