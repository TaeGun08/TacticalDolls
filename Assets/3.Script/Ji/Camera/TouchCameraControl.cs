using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Exoa.Cameras;
using Exoa.Common;
using UnityEngine;

public class TouchCameraControl : MonoBehaviour
{
    public CameraPerspective targetCamera; // TouchCamera
    private Coroutine resetCoroutine;
    
    private void LateUpdate()
    {
        if (BaseTouchInput.GetMouseWentUp(2))
        {
            targetCamera.ResetCameraY(); //휠 떨어질 때 카메라 각도 복구
        }
    }
}

    





