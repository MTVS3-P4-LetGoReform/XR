using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GotoFriendPark : MonoBehaviour
{
    public async void GotoPersonal(string userId = "jZ5qG2q6v1QQ29sjMi3MRZfxOp22")
    {
        Debug.Log("userid: "+ userId);
        var args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            //Scene = SceneRef.FromIndex(2),
            SessionName = userId,
            SessionProperties = new Dictionary<string, SessionProperty>()
            {
                {"UserId", userId},
            }
        };
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(args,3);
    }
}
