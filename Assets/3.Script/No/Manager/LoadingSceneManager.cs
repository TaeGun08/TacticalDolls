using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    private static string nextScene;
    
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private float loadDuration = 2f;
    
    private void Start()
    {
        LoadSceneAsync();
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
                progressText.text = "Complete!";
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