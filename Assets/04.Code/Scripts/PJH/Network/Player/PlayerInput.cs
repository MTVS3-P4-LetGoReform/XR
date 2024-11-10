using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : NetworkBehaviour
{
    public static Action<bool>OnChat;
    private bool _chatOn = false;
    
    public static Action<bool> MicMute;
    private bool _micOn = false;

    public static Action<bool> OnPlayerReady;
    private bool _onReady;
    
    public static Action<bool> OnGameStart;
    private bool _onGameStart;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (OnChat != null)
            {
                Chat();
            }
        }

        if (SceneManager.GetActiveScene().buildIndex != 2)
            return;
        
        if (Input.GetKeyDown(KeyCode.V) )
        {
            Mic();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("F5 키입력");
            Ready();
        }
        
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log("F6 키입력");
            StartGame();
        }
    }

    private void Chat()
    {
        if (_chatOn)
        {
            OnChat?.Invoke(false);
            _chatOn = false;
        }
        else
        {
            OnChat?.Invoke(true);
            _chatOn = true;
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

    private void Ready()
    {
        if (_onReady)
        {
            OnPlayerReady?.Invoke(false);
            _onReady = false;
        }
        else
        {
            OnPlayerReady?.Invoke(true);
            _onReady = true;
        }
    }

    private void StartGame()
    {
        if (_onGameStart)
        {
            OnGameStart?.Invoke(false);
            _onGameStart = false;
        }
        else
        {
            OnGameStart?.Invoke(true);
            _onGameStart = true;
        }
    }
}
