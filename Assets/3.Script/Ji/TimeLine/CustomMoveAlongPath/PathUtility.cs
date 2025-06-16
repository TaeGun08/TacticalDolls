using System.Collections.Generic;
using UnityEngine;

public static class PathUtility
{
    // 선형 보간으로 경로 따라 t 위치 반환
    public static Vector3 LerpAlongPath(List<Vector3> path, float t)
    {
        if (path == null || path.Count < 2)
            return Vector3.zero;

        float totalLength = 0f;
        List<float> segmentLengths = new();

        // 거리 누적
        for (int i = 1; i < path.Count; i++)
        {
            float segLen = Vector3.Distance(path[i - 1], path[i]);
            segmentLengths.Add(segLen);
            totalLength += segLen;
        }

        float targetDistance = totalLength * Mathf.Clamp01(t);
        float accumulated = 0f;

        for (int i = 0; i < segmentLengths.Count; i++)
        {
            float segLen = segmentLengths[i];

            if (accumulated + segLen >= targetDistance)
            {
                float segmentT = (targetDistance - accumulated) / segLen;
                return Vector3.Lerp(path[i], path[i + 1], segmentT);
            }

            accumulated += segLen;
        }

        return path[^1]; // 마지막 점 반환
    }
}