using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;
    public LoginHandler LoginHandler;
    public string nextSceneName = "LobbyScene"; // 다음 이동할 씬 이름
    
    //PlayerPrefs Keys
    private const string EmailKey = "UserEmail";
    private const string PasswordKey = "UserPassword";
    
    private void Awake()
    {
        Instance = this;
    }
    
    private async Task AutoLogin(string email, string password)
    {
        //자동 로그인 성공
        if(await FirebaseAccountManager.Instance.SignIn(email, password)) 
        {
            //로그아웃 버튼 활성화
            Debug.Log("Login success");
            LoadingSceneManager.LoadScene(nextSceneName);
        }
        else // 자동로그인 실패 PlayerPrefs는 있었지만 실제 계정이 없는 경우
        {
            PlayerPrefs.DeleteKey(EmailKey);
            PlayerPrefs.DeleteKey(PasswordKey);

            LoginHandler.LoginForm.SetActive(true);
        }
        
        LoginHandler.GameStartButton.interactable = false;
    }
    
    //화면을 클릭했을 때 로그인 확인
    public void OnClickedGameStartButtonPanelButton() 
    {
        // 0. 로그인 되어있다면 클릭시 씬이동
        if (FirebaseMainSession.Instance.FirebaseUser.UserData != null)
        {
            LoginHandler.GameStartButton.interactable = false;
            LoadingSceneManager.LoadScene(nextSceneName);
        }
        // 1. 로그인 안되어있음 && 이전 로그인 기록이 있음 => 자동 로그인 시도
        else if (PlayerPrefs.HasKey(EmailKey) && PlayerPrefs.HasKey(PasswordKey))
        {
            LoginHandler.GameStartButton.interactable = false;
            
            // 자동 로그인 시도
            AutoLogin(PlayerPrefs.GetString(EmailKey), PlayerPrefs.GetString(PasswordKey)).ContinueWithOnMainThread(
                task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        LoginHandler.GameStartButton.interactable = false;
                    }
                });
        }
        // 3. 모두 아니라면 로그인 창을 출력한다.
        else
        {
            LoginHandler.LoginForm.SetActive(true);
            LoginHandler.GameStartButton.gameObject.SetActive(false);
        }
    }
    
    public void OnClickedSignOutButton() //화면을 덮는 버튼 위에 위치해야 한다.
    {
        if (FirebaseMainSession.Instance.FirebaseUser.UserData != null) //클릭시 세션에 로그인정보가 있으면
        {
            FirebaseAccountManager.Instance.SignOut(); //로그아웃
            
            PlayerPrefs.DeleteKey(EmailKey);
            PlayerPrefs.DeleteKey(PasswordKey);
            // SignOutButton.gameObject.SetActive(false);
            // popupLogin.gameObject.SetActive(true);
        }
        
        //GameStartButtonPanel.interactable = true;
    }
    
    public void ReloadMainMenuScene()
    {
        if (FirebaseMainSession.Instance.FirebaseUser.UserData != null)
        {
            //SignOutButton.gameObject.SetActive(true);
        }
    }
}
