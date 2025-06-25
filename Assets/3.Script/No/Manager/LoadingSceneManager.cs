using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class LoadingSceneManager : MonoBehaviour
{
    private static string nextScene;
    
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private float loadDuration = 2f;
    [SerializeField] private TMP_Text notIfyText;
    [SerializeField] private Image notifyImg;
    [SerializeField] private float rotateSpeed = 180f;
    
    private List<String> Notify = new List<String>
    {
        "AI 전술 인형은 감정을 느끼지 못하지만, 가끔 꿈을 꾼다고 합니다.",
        "클루카이는 지난주에도 커피를 43잔 마셨습니다.",
        "상관없지만, 수오미는 아직도 냉동피자에 집착 중입니다.",
    };

    private int random;
    
    private void Start()
    {
        random = Random.Range(0, Notify.Count);
        notIfyText.text = Notify[random];
        
        LoadSceneAsync();
    }

    private void Update()
    {
        notifyImg.transform.Rotate(Vector3.forward, -rotateSpeed * Time.deltaTime);
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private async void LoadSceneAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        
        await StartLoading();
        
        op.allowSceneActivation = true;
    }
    
    private async Task StartLoading()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        
        slider.value = 0;
        DOTween.To(() => slider.value, x => slider.value = x, 1f, loadDuration)
            .SetEase(Ease.Linear)
            .OnUpdate(UpdateProgressText)
            .OnComplete(() =>
            {
                tcs.SetResult(true);
            });
        
        await tcs.Task;
    }
    
    private void UpdateProgressText()
    {
        int progress = Mathf.RoundToInt(slider.value * 100);
        progressText.text = $"{progress}%";
    }
}