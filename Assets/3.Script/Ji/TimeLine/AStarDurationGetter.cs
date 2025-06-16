using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarDurationGetter : MonoBehaviour
{
    
    public float ReturnAStarDuration(List<Vector2Int> path)
    {
        float totalDistance = 0f;
        for (int i = 1; i < path.Count; i++)
        {
            totalDistance += Vector2Int.Distance(path[i - 1], path[i]);
        }
        
        float speed = 2.0f; // 고정된 이동 속도 (예: 2m/s)
        float duration = totalDistance / speed;
        
        return duration;
    }
}
