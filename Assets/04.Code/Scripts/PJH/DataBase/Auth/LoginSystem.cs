using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static RealtimeDatabase;
public class LoginSystem : MonoBehaviour
{
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    public TMP_InputField signupEmail;
    public TMP_InputField signupPassword;
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
        string e = signupEmail.text;
        string p = signupPassword.text;
        string n = nickname.text;
        
        FirebaseAuthManager.Instance.CreateAccount(e,p,n);
    }

    public void Login()
    {
        FirebaseAuthManager.Instance.Login(loginEmail.text,loginPassword.text);
    }

    public void LogOut()
    {
        FirebaseAuthManager.Instance.LogOut();
    }
}
