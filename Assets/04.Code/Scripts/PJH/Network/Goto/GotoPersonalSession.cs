using System;
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
            GotoPersonal();
        }
    }

    private async void GotoPersonal(string name = "test")
    {
        var args = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = name
        };
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(args,2);
        _interact = true;
    }
}
