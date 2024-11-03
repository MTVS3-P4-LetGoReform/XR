using System;
using TMPro;
using UnityEngine;
using static RealtimeDatabase;
public class LoginSystem : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_InputField nickname;

    public TMP_Text outputText;

    private void Start()
    {
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.Init();
        InitializeFirebase();
    }

    private void OnChangedState(bool sign)
    {
        outputText.text = sign ? "Login : " : "Logout : ";
        outputText.text += FirebaseAuthManager.Instance.UserId;
    }

    public void Create()
    {
        string e = email.text;
        string p = password.text;
        string n = nickname.text;
        
        FirebaseAuthManager.Instance.Create(e,p,n);
    }

    public void Login()
    {
        FirebaseAuthManager.Instance.Login(email.text,password.text);
    }

    public void LogOut()
    {
        FirebaseAuthManager.Instance.LogOut();
    }
}
