using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CustomMoveAlongPathBehaviour : PlayableBehaviour
{
    public List<Vector3> pathPoints = new List<Vector3>();
    public float speed = 2.0f;

    public Vector3 startingPosition;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (playerData is not Transform target || pathPoints == null || pathPoints.Count < 2)
            return;

        float t = (float)(playable.GetTime() / playable.GetDuration());
        Vector3 pos = PathUtility.LerpAlongPath(pathPoints, t);
        target.position = pos;
    }

    public float GetEstimatedDuration()
    {
        float totalDistance = 0f;
        for (int i = 1; i < pathPoints.Count; i++)
        {
            totalDistance += Vector3.Distance(pathPoints[i - 1], pathPoints[i]);
        }
        return speed > 0 ? totalDistance / speed : 0;
    }
}
