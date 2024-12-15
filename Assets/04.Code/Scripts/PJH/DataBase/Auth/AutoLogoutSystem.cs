using UnityEngine;

public class AutoLogoutSystem : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        Logout();
    }

    private async void Logout()
    {
        Debug.Log("Logging out...");
        await FirebaseAuthManager.Instance.LogOutAsync();
    }
}
