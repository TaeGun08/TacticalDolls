using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineClipModifier : MonoBehaviour
{
    public PlayableDirector director;
    public TimelineAsset timelineAsset;

    // 교체할 클립(예: AnimationPlayableAsset, ActivationPlayableAsset, 커스텀 등)
    public TimelineAsset replacementAsset;

    void Start()
    {
        ModifyAllTimelineClips();
    }

    void ModifyAllTimelineClips()
    {
        if (director == null || timelineAsset == null)
        {
            Debug.LogWarning("PlayableDirector 또는 TimelineAsset이 비어 있습니다.");
            return;
        }

        // 모든 트랙 순회
        foreach (TrackAsset track in timelineAsset.GetOutputTracks())
        {
            // 클립이 있는 트랙만 처리
            foreach (TimelineClip clip in track.GetClips())
            {
                // 원하는 조건에 맞는 클립을 찾거나 수정
                Debug.Log($"new asset 타입: {replacementAsset.GetType().Name}, old타입: {clip.asset.GetType().Name}");
                
                // ✏️ 예시: 특정 타입의 클립만 바꾸기 (선택 사항)
                // if (clip.asset is CustomPlayableAsset customAsset) { ... }
                
                if (replacementAsset != null && clip.asset.GetType() == replacementAsset.GetType())
                {
                    Debug.Log("111");
                    clip.duration = 3;
                    clip.asset = replacementAsset;
                }
            }
        }

        // 그래프 재구성
        director.RebuildGraph();

        // 필요 시 재생
        director.Play();
    }
}