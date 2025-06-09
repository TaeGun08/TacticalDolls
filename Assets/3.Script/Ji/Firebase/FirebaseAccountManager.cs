using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public class FirebaseAccountManager : MonoBehaviour
{
    public static FirebaseAccountManager Instance { get; private set; }

    private FirebaseAuth auth;
    private FirebaseApp app;
    private bool isInitialized = false; //초기화 상태 확인용 bool
    private TaskCompletionSource<bool> resultTcs; //종료 시점 판단을 위한 Tcs

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await FirebaseApp.CheckAndFixDependenciesAsync();

        app = await LoadFirebaseAppFromStreamingAssetsAsync(Guid.NewGuid().ToString());

        if (app == null) return;

        auth = FirebaseAuth.GetAuth(app);
        FirestoreManager.Instance.InitializeFirebase(app);

        isInitialized = true;
    }

    public async Task<FirebaseApp> LoadFirebaseAppFromStreamingAssetsAsync(string appName = "CustomApp")
    {
        const string jsonFileNameForMobile = "google-services.json";
        const string jsonFileNameForDesktop = "google-services-desktop.json";

        string jsonFileNameTarget = jsonFileNameForDesktop;
#if UNITY_ANDROID && !UNITY_EDITOR
        jsonFileNameTarget = jsonFileNameForMobile;
#endif
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileNameTarget);
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        await SendRequestAsync(www);

        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{jsonFileNameTarget} 읽기 실패: {www.error}");
            return null;
        }

        string jsonText = www.downloadHandler.text;
#else

        if (File.Exists(filePath) == false)
        {
            Debug.LogError($"{jsonFileNameTarget} 없음.");
            return null;
        }

        string jsonText = await File.ReadAllTextAsync(filePath);
#endif

        JObject root = JObject.Parse(jsonText);

        string projectId = root["project_info"]?["project_id"]?.ToString();
        string storageBucket = root["project_info"]?["storage_bucket"]?.ToString();
        string projectNumber = root["project_info"]?["project_number"]?.ToString();

        var client = root["client"]?[0];
        string appId = client?["client_info"]?["mobilesdk_app_id"]?.ToString();
        string apiKey = client?["api_key"]?[0]?["current_key"]?.ToString();

        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError($"{jsonFileNameTarget} 파일 확인 필요");
            return null;
        }

        AppOptions options = new AppOptions
        {
            ProjectId = projectId,
            StorageBucket = storageBucket,
            AppId = appId,
            ApiKey = apiKey,
            MessageSenderId = projectNumber
        };

        FirebaseApp app = FirebaseApp.Create(options, appName);
        return app;
    }

    public static async Task<UnityWebRequest> SendRequestAsync(UnityWebRequest request)
    {
        var asyncOp = request.SendWebRequest();

        while (!asyncOp.isDone)
            await Task.Yield();

        return request;
    }

    public Task<bool> CreateAccount(string email, string password, string nickname) //계정 생성
    {
        if (isInitialized.Equals(false))
        {
            Debug.LogError("Firebase is not initialized.");
            return null;
        }

        resultTcs = new TaskCompletionSource<bool>();

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError(task.Exception);
                return;
            }

            var result = task.Result;
            Firebase.Auth.FirebaseUser newUser = result.User;
            FirebaseMainSession.Instance.SetUserData(newUser, nickname);

            //회원가입 성공
            UpdateUserNickname(newUser, nickname); // Auth 닉네임 설정
            CreateUserDocument(newUser.UserId, email, nickname); // Firestore에도 닉네임 저장
        });

        return resultTcs.Task;
    }

    private void UpdateUserNickname(Firebase.Auth.FirebaseUser user, string nickname)
    {
        if (isInitialized.Equals(false))
        {
            Debug.LogError("Firebase is not initialized.");
            return;
        }

        UserProfile profile = new UserProfile
        {
            DisplayName = nickname
        };

        user.UpdateUserProfileAsync(profile);
    }

    private void CreateUserDocument(string uid, string email, string nickname)
    {
        if (isInitialized.Equals(false))
        {
            Debug.LogError("Firebase is not initialized.");
            return;
        }

        PlayerDataSample userData = new PlayerDataSample() //please fix
        {
            // Email = email,
            // NickName = nickname,
            // CreatedAt = Timestamp.GetCurrentTimestamp(),
            // Role = "user",
            // IsTutorialCompleted = false
            // //Freiends
        };

        FirestoreManager.Instance.WriteDataAsync<PlayerDataSample>(FirebaseCollections.Players, uid, userData)
            .ContinueWithOnMainThread(
                task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        Debug.LogError(task.Exception);
                        return;
                    }

                    resultTcs.TrySetResult(true);
                });
    }

    public async Task<bool> SignIn(string email, string password) //로그인
    {
        bool isSignIn = false;

        if (isInitialized.Equals(false))
        {
            Debug.LogError("Firebase is not initialized.");
            return isSignIn;
        }


        await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError(task.Exception);
                return;
            }
        
            isSignIn = true;
            var result = task.Result;
            Firebase.Auth.FirebaseUser user = result.User;
        
            FirestoreManager.Instance.ReadDataAsync<PlayerDataSample>(FirebaseCollections.Players, user.UserId)
                .ContinueWithOnMainThread(
                    task =>
                    {
                        if (task.IsCanceled || task.IsFaulted)
                        {
                            return;
                        }
        
                        // FirebaseMainSession.Instance.SetUserData(user, task.Result.NickName); //please fix
                    });
        });

        return isSignIn;
    }

    public void SignOut() //실행하는곳에서 login false 하기
    {
        auth.SignOut();
        FirebaseMainSession.Instance.SetUserData(null, null);
    }
}