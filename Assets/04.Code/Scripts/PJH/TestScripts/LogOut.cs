using UnityEngine;

public class LogOut : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            FirebaseAuthManager.Instance.LogOut();
        }
    }
}
