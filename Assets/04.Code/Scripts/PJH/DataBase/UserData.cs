using UnityEngine;

public class UserData : MonoBehaviour
{
    public static string UserName;
    public static string UserId;
    
    private void Start()
    {
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
    }
    
    private void OnChangedState(bool sign)
    {
        UserId += FirebaseAuthManager.Instance.UserId;
    }
}
