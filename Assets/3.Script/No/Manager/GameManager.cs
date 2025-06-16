using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PrefabsTable CharacterTable;
    public Button ExitButton;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        ExitButton.onClick.AddListener(OnExitButtonClicked);
    }
    
    private void OnExitButtonClicked()
    {
        PlayerManager.Instance.ResetCachedCharacterData();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // 빌드된 애플리케이션에서는 종료
    Application.Quit();
#endif
    }
}
