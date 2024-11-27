using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GotoPersonalSession : MonoBehaviour
{
    private bool _interact = true;
    public Canvas canvasHeartPing;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("닿았다");
        
        if (other.CompareTag("Player") && _interact && Input.GetKeyDown(KeyCode.E))
        {
            _interact = false;
            Debug.Log("눌렀다");
            canvasHeartPing.gameObject.SetActive(true);
        }

        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Escape))
        {
            canvasHeartPing.gameObject.SetActive(false);
        }
    }

    public void GotoPersonalOnClick()
    {
        GotoPersonal(UserData.Instance.UserName);
        canvasHeartPing.gameObject.SetActive(false);
    }
    

    private async void GotoPersonal(string userId)
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
