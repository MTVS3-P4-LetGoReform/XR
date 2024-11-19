using System;
using Unity.VisualScripting;
using UnityEngine;

public class UserData : MonoBehaviourSingleton<UserData>
{
    public string UserName;
    public string UserId;
    
    
    private void Start()
    {
        FirebaseAuthManager.Instance.Init();
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.NickName += OnChangedNickName;
        Debug.Log("이벤트 연결 완료");
    }
    
    
    private void OnChangedState(bool sign)
    {
        UserId += FirebaseAuthManager.Instance.UserId;
        
    }

    private void OnChangedNickName(string username)
    {
        UserName = username;
        Debug.Log("유저 데이터 : " + username);
    }
}
