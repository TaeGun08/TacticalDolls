using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginHandler : MonoBehaviour
{
    [Header("Panel Settings")]
    public GameObject JoinForm;
    public GameObject AlertForm;
    public TMP_Text alertInputField;
    
    [Header("Button Settings")]
    public Button LoginButton;
    public Button JoinButton;
    public Button backButton;
    public Button OpenJoinFormButton;

    [Header("Login Input Settings")]
    public TMP_InputField LId;
    public TMP_InputField LPassword;

    [Header("Join Input Settings")]
    public TMP_InputField INick;
    public TMP_InputField IId;
    public TMP_InputField IPassword;
    
    void Start()
    {
        LoginButton.onClick.AddListener(OnLoginButtonClicked);
        JoinButton.onClick.AddListener(OnJoinButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        OpenJoinFormButton.onClick.AddListener(()=> JoinForm.SetActive(true));
    }
    
    void OnLoginButtonClicked()
    {
        string userId = LId.text;
        string userPassword = LPassword.text;
        
        Debug.Log($"ID:{userId} / PW:{userPassword}");
        
        // 로그인 완료
        SceneManager.LoadScene("LobbyScene");
    }

    void OnJoinButtonClicked()
    {
        string useNickName = INick.text;
        string userId = IId.text;
        string userPassword = IPassword.text;

        Debug.Log($"NICK: {useNickName} / ID:{userId} / PW:{userPassword}");
        
        // 회원가입 성공
        AlertForm.SetActive(true);
        alertInputField.text = $"{useNickName} 회원 가입 성공";
        
        JoinForm.SetActive(false);

        StartCoroutine(TestCo());
    }
    
    void OnBackButtonClicked()
    {
        // 뒤로가기
        JoinForm.SetActive(false);
    }
    
    IEnumerator TestCo()
    {
        yield return new WaitForSeconds(2f);
        
        AlertForm.SetActive(false);
        alertInputField.text = $"";
    }
}
