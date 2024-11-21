using System;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    public string UserId => _user.UserId;

    public Action<bool> LoginState;
    public Action<string> NickName;

    public Action<string> OnLoginResult;
    public Action<string> OnSignUpResult;

    private FirebaseAuth _auth;
    private FirebaseUser _user;

    public void Init()
    {
        _auth = FirebaseAuth.DefaultInstance;
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
public void CreateAccount(string email, string password, string nickname)
{
    // 닉네임 중복 확인
    RealtimeDatabase.FindUserIdByUsername(nickname,
        onSuccess: existingUserId =>
        {
            if (string.IsNullOrEmpty(existingUserId))  // 닉네임이 중복되지 않으면
            {
                // Firebase Authentication을 통해 사용자 생성
                _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("회원가입 취소");
                        OnSignUpResult.Invoke("회원가입이 취소되었습니다.");
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        Debug.LogError("회원가입 실패: " + task.Exception);
                        OnSignUpResult.Invoke("회원가입에 실패했습니다." + task.Exception);
                        return;
                    }

                    FirebaseUser newUser = task.Result.User;
                    Debug.Log("회원가입 완료");

                    // 사용자 객체 생성
                    User user = new User(nickname, email, "default_profile_image_url", true, DateTimeOffset.UtcNow.AddHours(9).ToUnixTimeSeconds());

                    // 사용자 정보와 닉네임 저장
                    RealtimeDatabase.CreateUserWithUsername(newUser.UserId, user,
                        onSuccess: () =>
                        {
                            Debug.Log("사용자 정보 저장 완료");
                            OnSignUpResult.Invoke("회원가입이 완료되었습니다.");
                        },
                        onFailure: (exception) =>
                        {
                            Debug.LogError("사용자 정보 저장 실패: " + exception.Message);
                            OnSignUpResult.Invoke("사용자 정보 저장에 실패했습니다.");
                        });
                });
            }
            else
            {
                // 닉네임 중복 시 오류 처리
                Debug.LogError("이미 사용 중인 닉네임입니다.");
                OnSignUpResult.Invoke("이미 사용 중인 닉네임입니다.");
            }
        },
        onFailure: exception =>
        {
            // 닉네임 확인 중 오류 발생
            Debug.LogError("닉네임 확인 실패: " + exception.Message);
            OnSignUpResult.Invoke("닉네임 확인 중 오류가 발생했습니다.");
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
                OnLoginResult.Invoke("로그인이 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("로그인 실패");
                OnLoginResult.Invoke("로그인에 실패했습니다.");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("로그인 성공");

            RealtimeDatabase.GetUser(newUser.UserId, user =>
                {
                    if (user != null)
                    {
                        Debug.Log($"사용자 데이터 로드 성공: {user.name}, {user.email}");
                        NickName?.Invoke(user.name);
                        OnLoginResult.Invoke("로그인 성공");
                    }
                    else
                    {
                        NickName?.Invoke("알 수 없는 사용자");
                        Debug.Log("사용자 데이터를 찾을 수 없습니다.");
                        OnLoginResult.Invoke("이메일 또는 비밀번호를 잘못되었습니다.");

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
        RealtimeDatabase.UpdateUser(id + "/", updates);

        _auth.SignOut();
        Debug.Log("로그아웃");
    }
}
