using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

[Serializable]
public class MoveParticleTransformBehaviour : PlayableBehaviour
{
    bool m_FirstFrameHappened = false;
    public Transform targetLocation;  // 이동 위치
    public GameObject particleObject; // 파티클 프리팹 or 인스턴스
    private bool isParticleActive = false;
}
