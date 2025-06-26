using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Exoa.Cameras;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;
using UnityEngine.Sequences.Timeline;
using CinemachineBlendDefinition = Cinemachine.CinemachineBlendDefinition;

public class InGameCameraPD : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera topViewCam;
    [SerializeField] private CameraPerspective topViewCamAsTouchCam;
    [SerializeField] private CinemachineVirtualCamera middleZoomCamera;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private GridBehavior gridBehavior;
    
    private void Start()
    {
        // middleZoomCamera.enabled = false;
        gridBehavior.callback.startMove += CutToMiddleCamera;
        gridBehavior.callback.onCompleteMove += BlendBackToTopViewAfterAction;
    }

    private void OnDisable()
    {
        gridBehavior.callback.startMove -= CutToMiddleCamera;
        gridBehavior.callback.onCompleteMove -= BlendBackToTopViewAfterAction;
    }
    
    private void CutToMiddleCamera(Transform character)
    {
        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f); //컷으로 전환되는 효과 세팅
        
        // 탑뷰 카메라 비활성화, MiddleZoomCamera 활성화
        topViewCam.enabled = false;
        middleZoomCamera.Follow = character.transform;
        middleZoomCamera.enabled = true;
        
        // MiddleZoomCamera가 현재 바라보게 우선순위 설정
        topViewCam.Priority = 10;
        middleZoomCamera.Priority = 20;
    }
    
    private void BlendBackToTopViewAfterAction(Transform character)
    {
        // 블렌딩 세팅
        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1f);
        
        // 탑뷰 카메라 활성화, MiddleZoomCamera 비활성화
        middleZoomCamera.enabled = false;
        middleZoomCamera.Follow = null;
        topViewCam.enabled = true;
        
        topViewCam.Priority = 20;
        middleZoomCamera.Priority = 10;
        topViewCamAsTouchCam.MoveCameraTo(character.position);
    }
}
