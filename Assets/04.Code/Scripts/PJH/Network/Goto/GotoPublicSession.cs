using UnityEngine;

public class GotoPublicSession : MonoBehaviour
{
    public async void GotoPublic()
    {
        Cursor.lockState = CursorLockMode.Locked;
        await RunnerManager.Instance.JoinPublicSession();
    }
}
