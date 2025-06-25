using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    public CinemachineVirtualCamera MiddleZoomCamera;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private GridBehavior bottomViewCam;
    
    private void Start()
    {
        bottomViewCam.callback.startMove += CutToMiddleCamera;
        bottomViewCam.callback.onCompleteMove += BlendBackToTopViewAfterAction;
    }

    private void OnDisable()
    {
        bottomViewCam.callback.startMove -= CutToMiddleCamera;
        bottomViewCam.callback.onCompleteMove -= BlendBackToTopViewAfterAction;
    }
    
    private void CutToMiddleCamera(CinemachineVirtualCamera characterMiddleZoomCamera)
    {
        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f); //컷으로 전환되는 효과 세팅
        MiddleZoomCamera = characterMiddleZoomCamera;
        
        // 탑뷰 카메라 비활성화, MiddleZoomCamera 활성화
        topViewCam.enabled = false;
        MiddleZoomCamera.enabled = true;
        
        // MiddleZoomCamera가 현재 바라보게 우선순위 설정
        topViewCam.Priority = 10;
        MiddleZoomCamera.Priority = 20;
    }
    
    private void BlendBackToTopViewAfterAction()
    {
        // 블렌딩 세팅
        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1f);
        
        // 탑뷰 카메라 활성화, MiddleZoomCamera 비활성화
        MiddleZoomCamera.enabled = false;
        topViewCam.enabled = true;
        
        topViewCam.Priority = 20;
        MiddleZoomCamera.Priority = 10;
    }
}
