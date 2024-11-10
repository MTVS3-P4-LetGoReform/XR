using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : NetworkBehaviour
{
    public delegate void ChatEvent();
    public static event ChatEvent OnChat;
 
    public static Action<bool> MicMute;

    private bool _micOn = false;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (OnChat != null)
            {
                OnChat?.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.V) && SceneManager.GetActiveScene().name == "Proto_PlayScene")
        {
            Mic();
        }
    }

    private void Mic()
    {
        if (_micOn)
        {
            MicMute?.Invoke(false);
            _micOn = false;
        }
        else
        {
            MicMute?.Invoke(true);
            _micOn = true;
        }
    }
}
