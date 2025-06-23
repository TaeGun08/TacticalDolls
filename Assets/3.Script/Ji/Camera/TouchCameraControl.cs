using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Exoa.Cameras;
using Exoa.Common;
using UnityEngine;

public class TouchCameraControl : MonoBehaviour
{
    public CameraPerspective targetCamera; // TouchCamera 자체
    public float originalPitch = 45f; // 초기 시점의 pitch 각도
    public float returnDelay = 2f;
    public float returnDuration = 1f;
    
    private Coroutine resetCoroutine;

    public Transform[] targets;
    int index = 0;


    private void LateUpdate()
    {
        if (BaseTouchInput.GetMouseWentUp(2))
        {
            targetCamera.ResetCameraY(); //휠 떨어질 때 카메라 각도 복구
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            targetCamera.ResetCamera();
            targetCamera.MoveCameraTo(targets[index%4].position);
            index++;
        }
    }

    private IEnumerator ResetPitchAfterDelay()
    {
        // targetCamera.ResetCamera();

        // Quaternion rotation45Pitch = Quaternion.Euler(45f, 0f, 0f);
        targetCamera.ResetCameraY();
        // targetCamera.Init();
        // targetCamera.StopFollow();
        // targetCamera.FocusCamera(targetCamera.transform.position, targetCamera.initDistance, rotation45Pitch);
        yield return new WaitForSeconds(returnDelay);
    }
}

    





