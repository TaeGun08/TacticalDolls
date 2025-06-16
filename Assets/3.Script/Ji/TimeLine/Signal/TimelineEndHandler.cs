using UnityEngine;
using UnityEngine.Playables;

public class TimelineEndHandler : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    public void OnTimelineEnd()
    {
        if (director == null)
        {
            Debug.LogWarning("PlayableDirector가 설정되지 않았습니다.");
            return;
        }

        // 방법 1: 재생 중단
        director.Stop();

        // 방법 2 (선택적): 타임라인 연결도 끊음
        // director.playableAsset = null;

        Debug.Log("🎬 타임라인 종료 후 제어 해제됨");
    }
}