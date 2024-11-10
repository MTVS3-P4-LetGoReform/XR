using System;
using Unity.VisualScripting;
using UnityEngine;

public class UserData : MonoBehaviourSingleton<UserData>
{
    public string UserName = "testName";
    public string UserId;
    
    
    private void Start()
    {
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.NickName += OnchangedNickName;
    }
    
    
    private void OnChangedState(bool sign)
    {
        UserId += FirebaseAuthManager.Instance.UserId;
        
    }

    private void OnchangedNickName(bool sign)
    {
        UserName = FirebaseAuthManager.Instance.UserName;
    }
}
