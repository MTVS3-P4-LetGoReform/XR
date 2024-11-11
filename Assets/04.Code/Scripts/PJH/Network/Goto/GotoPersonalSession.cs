using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GotoPersonalSession : MonoBehaviour
{
    private bool _interact = true;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("닿았다");
        
        if (other.CompareTag("Player") && _interact && Input.GetKeyDown(KeyCode.E))
        {
            _interact = false;
            Debug.Log("눌렀다");
            GotoPersonal(UserData.Instance.UserName);
        }
    }

    private async void GotoPersonal(string userId = "TestId")
    {
        Debug.Log("userid: "+ userId);
        var args = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            //Scene = SceneRef.FromIndex(2),
            SessionName = userId,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "UserId", userId },
            }
        };
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(args,3);
        _interact = true;
    }
    
}
