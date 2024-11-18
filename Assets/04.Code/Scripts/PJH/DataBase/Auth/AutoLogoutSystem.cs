using UnityEngine;

public class AutoLogoutSystem : MonoBehaviour
{
    void OnApplicationQuit()
    {
        Logout();
    }

    void Logout()
    {
        Debug.Log("Logging out...");
        FirebaseAuthManager.Instance.LogOut();
    }
}
