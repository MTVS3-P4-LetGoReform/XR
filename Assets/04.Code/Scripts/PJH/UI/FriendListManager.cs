using UnityEngine;

public class FriendListManager : MonoBehaviour
{
    private Canvas _friendListCanvas;
    void Start()
    {
        _friendListCanvas = GetComponent<Canvas>();
        PlayerInput.OnMessenger += OpenMessenger;
    }

    private void OpenMessenger(bool open)
    {
        _friendListCanvas.enabled = open;
    }
}
