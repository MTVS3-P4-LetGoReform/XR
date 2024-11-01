using System;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public static string UserName;

    
    private void Start()
    {
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
    }
    
    private void OnChangedState(bool sign)
    {
        UserName += FirebaseAuthManager.Instance.UserId;
    }
}
