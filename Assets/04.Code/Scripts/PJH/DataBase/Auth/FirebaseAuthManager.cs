using System;
using UnityEngine;
using Firebase.Auth;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    public string UserId => _user?.UserId;

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
    public async UniTask CreateAccountAsync(string email, string password, string nickname)
    {
        try
        {
            var existingUserId = await RealtimeDatabase.FindUserIdByUsernameAsync(nickname);
            
            if (!string.IsNullOrEmpty(existingUserId))
            {
                OnSignUpResult?.Invoke("이미 사용 중인 닉네임입니다.");
                return;
            }

            var authResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var newUser = authResult.User;
            
            var user = new User(
                nickname, 
                email, 
                "default_profile_image_url", 
                true, 
                DateTimeOffset.UtcNow.AddHours(9).ToUnixTimeSeconds()
            );

            await RealtimeDatabase.CreateUserWithUsernameAsync(newUser.UserId, user);
            Debug.Log("회원가입 및 사용자 정보 저장 완료");
            OnSignUpResult?.Invoke("회원가입이 완료되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"회원가입 실패: {e.Message}");
            OnSignUpResult?.Invoke($"회원가입에 실패했습니다: {e.Message}");
        }
    }

    /// <summary>
    /// 로그인 후 사용자 데이터를 불러옵니다.
    /// </summary>
    public async UniTask LoginAsync(string email, string password)
    {
        try
        {
            var authResult = await _auth.SignInWithEmailAndPasswordAsync(email, password);
            var newUser = authResult.User;
            Debug.Log("로그인 성공");

            var user = await RealtimeDatabase.GetUserAsync(newUser.UserId);
            if (user != null)
            {
                Debug.Log($"사용자 데이터 로드 성공: {user.name}, {user.email}");
                NickName?.Invoke(user.name);
                OnLoginResult?.Invoke("로그인 성공");

                var updates = new Dictionary<string, object>
                {
                    { "onlineStatus", true },
                    { "lastLogin", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
                };
                await RealtimeDatabase.UpdateDataAsync($"users/{newUser.UserId}", updates);
            }
            else
            {
                NickName?.Invoke("알 수 없는 사용자");
                OnLoginResult?.Invoke("이메일 또는 비밀번호가 잘못되었습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"로그인 실패: {e.Message}");
            OnLoginResult?.Invoke("로그인에 실패했습니다.");
        }
    }

    /// <summary>
    /// 로그아웃을 수행합니다.
    /// </summary>
    public async UniTask LogOutAsync()
    {
        try
        {
            var updates = new Dictionary<string, object>
            {
                { "onlineStatus", false },
                { "lastLogin", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
            };

            if (UserData.Instance.UserId != null)
            {
                await RealtimeDatabase.UpdateDataAsync($"users/{UserData.Instance.UserId}", updates);
            }
            
            _auth.SignOut();
            Debug.Log("로그아웃 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"로그아웃 실패: {e.Message}");
            throw;
        }
    }
}
