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
    
    public static Action<bool> OnMessenger;
    private bool _onMessenger;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (OnChat != null)
            {
                Chat();
            }
        }

        if (Input.GetKeyDown(KeyCode.P) && SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("P 키입력");
            Messenger();
        }
        
        if (SceneManager.GetActiveScene().buildIndex != 2)
            return;
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("V 키입력");
            Mic();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("F1 키입력");
            Ready();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("F2 키입력");
            StartGame();
        }
    }
    
    private void Messenger()
    {
        if (_onMessenger)
        {
            OnMessenger?.Invoke(false);
            _onMessenger = false;
        }
        else
        {
            OnMessenger?.Invoke(true);
            _onMessenger = true;
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
            Debug.Log("마이크 킴");
            MicMute?.Invoke(false);
            _micOn = false;
        }
        else
        {
            Debug.Log("마이크 끔");
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
