using UnityEngine;

public class GotoLobbySession : MonoBehaviour
{
    public async void JoinLobby()
    {
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.JoinLobby();
    }
}
