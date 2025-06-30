using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginHandler : MonoBehaviour
{
    [Header("Panel Settings")]
    public GameObject LoginForm;
    public GameObject JoinForm;
    public TMP_Text startText;
    
    [Header("Button Settings")]
    public Button GameStartButton;
    public Button LoginButton;
    public Button JoinButton;
    public Button backButton;
    public Button OpenJoinFormButton;

    [Header("Login Input Settings")]
    public TMP_InputField LEmail;
    public TMP_InputField LPassword;

    [Header("Join Input Settings")]
    public TMP_InputField INick;
    public TMP_InputField IEmail;
    public TMP_InputField IPassword;
    
    public bool isLoggedIn { get; private set; } = false;

    [Header("Auto Login Toggle")]
    [SerializeField] private Toggle rememberToggle;
    
    //PlayerPrefs Keys
    private const string EmailKey = "UserEmail";
    private const string PasswordKey = "UserPassword";
    
    private void Awake()
    {
        // 로그인 입력 제어
        LEmail.characterLimit = 30;
        LPassword.characterLimit = 15;

        LEmail.contentType = TMP_InputField.ContentType.Custom;
        LEmail.onValidateInput += BlockKoreanInput;
        LPassword.contentType = TMP_InputField.ContentType.Custom;
        LPassword.onValidateInput += BlockKoreanInput;
        
        // 회원가입 입력 제어
        INick.characterLimit = 6;
        IEmail.characterLimit = 30;
        IPassword.characterLimit = 15;
        
        INick.contentType = TMP_InputField.ContentType.Custom;
        INick.onValidateInput += BlockKoreanInput;
        IEmail.contentType = TMP_InputField.ContentType.Custom;
        IEmail.onValidateInput += BlockKoreanInput;
        IPassword.contentType = TMP_InputField.ContentType.Custom;
        IPassword.onValidateInput += BlockKoreanInput;
    }
    
    
    void Start()
    {
        SoundManager.Instance.PlayBgm("maou_bgm_cyber42");
        LoginButton.onClick.AddListener(OnLoginButtonClicked);
        JoinButton.onClick.AddListener(OnJoinButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        OpenJoinFormButton.onClick.AddListener(()=> JoinForm.SetActive(true));
        GameStartButton.onClick.AddListener(LoginManager.Instance.OnClickedGameStartButtonPanelButton);
        
        // 시작 시 텍스트 투명하게 만들기
        Color c = startText.color;
        c.a = 0;
        startText.color = c;

        // 0 → 1 → 0 페이드 인 & 아웃
        startText.DOFade(1f, 2.0f)
            .SetLoops(-1, LoopType.Yoyo) 
            .SetEase(Ease.InOutSine);
    }
    
    // 입력 값 체크
    private char BlockKoreanInput(string text, int charIndex, char addedChar)
    {
        if (IsKorean(addedChar))
        {
            return '\0'; // 입력 무시
        }
        return addedChar;
    }
    
    private bool IsKorean(char c)
    {
        // 한글 음절: 가 ~ 힣
        if (c >= 0xAC00 && c <= 0xD7A3) return true;

        // 한글 자모 (초성/중성/종성)
        if (c >= 0x1100 && c <= 0x11FF) return true;   // 자모 (Hangul Jamo)
        if (c >= 0x3130 && c <= 0x318F) return true;   // 호환 자모 (Compatibility Jamo)
    
        return false;
    }
    
    private async void OnLoginButtonClicked()
    {
        string userEmail = LEmail.text;
        string userPassword = LPassword.text;
     
        // 로그인 요청
        if (await FirebaseAccountManager.Instance.SignIn(userEmail, userPassword)) //return bool
        {
            //로그인 성공
            isLoggedIn = true;
            
            //PlayerPrefs를 이용한 자동 로그인 세팅
            if (rememberToggle.isOn)
            {
                PlayerPrefs.SetString(EmailKey, userEmail);
                PlayerPrefs.SetString(PasswordKey, userPassword);
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.DeleteKey(EmailKey);
                PlayerPrefs.DeleteKey(PasswordKey);
            }
            
            LoadingSceneManager.LoadScene(LoginManager.Instance.nextSceneName);
        }
        else
        {
            //로그인 실패
        }
    }

    void OnJoinButtonClicked()
    {
        string useNickName = INick.text;
        string userEmail = IEmail.text;
        string userPassword = IPassword.text;
        
        // 회원가입 성공
        var res = FirebaseAccountManager.Instance.CreateAccount(userEmail, userPassword, useNickName);

        if (res != null)
        {
            // 회원가입 성공
            OnBackButtonClicked();
        }
        else
        {
            // 회원가입 실패
        }
    }
    
    public void OnBackButtonClicked()
    {
        // 뒤로가기
        JoinForm.SetActive(false);
        INick.text = "";
        IEmail.text = "";
        IPassword.text = "";
    }
}
