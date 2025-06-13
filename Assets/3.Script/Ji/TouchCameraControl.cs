using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Exoa.Cameras;
using UnityEngine;

public class TouchCameraControl : MonoBehaviour
{
    public CameraPerspective targetCamera; // TouchCamera 자체
    public float originalPitch = 45f; // 초기 시점의 pitch 각도
    public float returnDelay = 2f;
    public float returnDuration = 1f;
    
    private Coroutine resetCoroutine;
    private Tween pitchTween;

    void Update()
    {
        if (Input.GetMouseButton(2)) // 마우스 우클릭 회전 중
        {
            if (resetCoroutine != null)
            {
                // StopCoroutine(resetCoroutine);
                resetCoroutine = null;
            }

            if (pitchTween != null && pitchTween.IsActive())
                pitchTween.Kill();
        }
        else
        {
            if (resetCoroutine == null)
            {
                resetCoroutine = StartCoroutine(ResetPitchAfterDelay());
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            targetCamera.Init();
        }
    }

    private IEnumerator ResetPitchAfterDelay()
    {
        // targetCamera.ResetCamera();

        Quaternion rotation45Pitch = Quaternion.Euler(45f, 0f, 0f);
        targetCamera.Init();
        targetCamera.StopFollow();
        targetCamera.FocusCamera(targetCamera.transform.position, targetCamera.initDistance, rotation45Pitch);
        yield return new WaitForSeconds(returnDelay);
    }
}


