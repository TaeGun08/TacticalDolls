using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MoveTransformBehaviour : PlayableBehaviour
{
    public Transform startTransform;
    public Transform endTransform;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var transform = playerData as Transform;
        if (transform == null || startTransform == null || endTransform == null)
            return;

        double time = playable.GetTime();
        double duration = playable.GetDuration();
        float t = (float)(time / duration);

        transform.position = Vector3.Lerp(startTransform.position, endTransform.position, t);
    }
}