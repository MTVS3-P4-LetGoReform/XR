using System;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{ 
    public string UserId => _user.UserId;
    public string UserName => _name;
    
    public Action<bool> LoginState;
    public Action<bool> NickName;
    
    private FirebaseAuth _auth;
    private FirebaseUser _user;
    private string _name;
    
    public void Init()
    {
        _auth = FirebaseAuth.DefaultInstance;
        // 임시 처리
        if (_auth.CurrentUser != null)
        {
            LogOut();
        }

        _auth.StateChanged += OnChanged;
    }
    
    //
    private void OnChanged(object sender, EventArgs e)
    {
        if (_auth.CurrentUser != _user)
        {
            bool signed = (_auth.CurrentUser != _user && _auth.CurrentUser != null);
            if (!signed && _user != null)
            {
                Debug.Log("로그아웃");
                LoginState?.Invoke(false);
            }

            _user = _auth.CurrentUser;
            if (signed)
            {
                Debug.Log("로그인");
                LoginState?.Invoke(true);
            }
        }
    }
    
    /// <summary>
    /// 회원가입 후 사용자 정보를 데이터베이스에 저장합니다.
    /// </summary>
    public void CreateAccount(string email, string password,string nickname)
    {
        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("회원가입 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("회원가입 실패" + task.Exception);
                return;
            }
            
            FirebaseUser newUser = task.Result.User;
            Debug.Log("회원가입 완료");

            User user = new User(nickname, email, "default_profile_image_url", true, DateTimeOffset.UtcNow.AddHours(9).ToUnixTimeSeconds());
            RealtimeDatabase.CreateUser(newUser.UserId, user,
                onSuccess: () => Debug.Log("사용자 정보 저장 완료"),
                onFailure: (exception) => Debug.LogError("사용자 정보 저장 실패: " + exception.Message));
            
        });
    }

    /// <summary>
    /// 로그인 후 사용자 데이터를 불러옵니다.
    /// </summary>
    public void Login(string email, string password)
    {
        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("로그인 실패");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("로그인 성공");
            
            RealtimeDatabase.GetUser(newUser.UserId, user =>
                {
                    if (user != null)
                    {
                        Debug.Log($"사용자 데이터 로드 성공: {user.name}, {user.email}");
                        _name = user.name;
                        NickName?.Invoke(true);
                        var sceneName = SceneUtility.GetScenePathByBuildIndex(1);
                       // SceneLoadManager.Instance.LoadScene(sceneName);
                    }
                    else
                    {
                        Debug.Log("사용자 데이터를 찾을 수 없습니다.");
                        NickName?.Invoke(false);
                    }
                },
                onFailure: (exception) => Debug.LogError("사용자 데이터 로드 실패: " + exception.Message));
            var updates = new Dictionary<string, object>
            {
                { "onlineStatus", true },
                { "lastLogin", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
            };
            RealtimeDatabase.UpdateUser(newUser.UserId, updates);
        });
    }

    public void LogOut()
    {
       
        var id = UserData.Instance.UserId;
        
        var updates = new Dictionary<string, object>
        {
            { "onlineStatus", false },
            { "lastLogin", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
        };
        RealtimeDatabase.UpdateUser(id+"/", updates);
        
        _auth.SignOut();
        Debug.Log("로그아웃");
    }
}
