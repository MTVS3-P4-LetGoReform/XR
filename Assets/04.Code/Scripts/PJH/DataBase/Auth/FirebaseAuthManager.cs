using System;
using UnityEngine;
using Firebase.Auth;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{ 
    public string UserId => _user.UserId;

    public Action<bool> LoginState;
    
    private FirebaseAuth _auth;
    private FirebaseUser _user;
    
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
    
    public void Create(string email, string password)
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
                Debug.LogError("회원가입 실패");
                return;
            }

            AuthResult result = task.Result;
            FirebaseUser newUser = result.User;
            Debug.Log("회원가입 완료");
        });
    }

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

            AuthResult result = task.Result;
            FirebaseUser newUser = result.User;
            Debug.Log("로그인 완료");
        });
    }

    public void LogOut()
    {
        _auth.SignOut();
        Debug.Log("로그아웃");
    }
}
    /*private static FirebaseAuthManager instance = null;

    public static FirebaseAuthManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FirebaseAuthManager();
            }

            return instance;
        }
    }
    */
