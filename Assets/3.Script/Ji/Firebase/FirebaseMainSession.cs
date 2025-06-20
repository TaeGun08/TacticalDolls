using UnityEngine;

public class FirebaseUser
{
    public Firebase.Auth.FirebaseUser UserData { get; set; }
    public string Username { get; set; }
}

// 로그인 유저 정보 계속 유지
public class FirebaseMainSession : MonoBehaviour
{
    public static FirebaseMainSession Instance { get; private set; }

    public FirebaseUser FirebaseUser { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        FirebaseUser = new FirebaseUser();
    }
    
    public void SetUserData(Firebase.Auth.FirebaseUser user, string username)
    {
        FirebaseUser.UserData = user;
        FirebaseUser.Username =  username;

        if (user != null) //디버그용
        {
            Debug.Log($"MainSystem UserId ::: {FirebaseUser.UserData.UserId}");
            Debug.Log($"MainSystem userName ::: {username}");
        }
    }
}