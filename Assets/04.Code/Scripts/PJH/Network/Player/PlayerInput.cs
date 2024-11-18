using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : NetworkBehaviour
{
    public static event Action<bool> OnMouse;
    public static event Action<bool> OnChat;
    public static event Action<bool> MicMute;
    public static event Action<bool> OnPlayerReady;
    public static event Action<bool> OnGameStart;
    public static event Action<bool> OnMessenger;

    private bool _mouseOn = false;
    private bool _chatOn = false;
    private bool _micOn = false;
    private bool _onReady = false;
    private bool _onGameStart = false;
    private bool _onMessenger = false;

    public PlayerStatus PlayerStatus { get; private set; }

    public override void Spawned()
    {
        PlayerStatus = GetComponentInParent<PlayerStatus>();
        if (PlayerStatus == null)
        {
            Debug.LogError("PlayerStatus가 부모 객체에 없습니다.");
        }
    }

    private void Update()
    {
        HandleGeneralInput();
        HandleSceneSpecificInput();
    }

    private void HandleGeneralInput()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            PlayerStatus?.Reword(true);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ToggleChat();
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMouse();
        }
    }

   
    private void HandleSceneSpecificInput()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MessengerScene")
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                //Debug.Log("P 키 입력");
                ToggleMessenger();
            }
        }
        else if (sceneName == "GameScene")
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                //Debug.Log("V 키 입력");
                ToggleMic();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                //Debug.Log("F1 키 입력");
                ToggleReady();
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                //Debug.Log("F2 키 입력");
                ToggleGameStart();
            }
        }
    }
    
    private void ToggleMouse()
    {
        _mouseOn = !_mouseOn;
        OnMouse?.Invoke(_mouseOn);
    }
    
    private void ToggleChat()
    {
        _chatOn = !_chatOn;
        OnChat?.Invoke(_chatOn);
        Debug.Log(_chatOn ? "채팅 활성화" : "채팅 비활성화");
    }

    private void ToggleMessenger()
    {
        _onMessenger = !_onMessenger;
        OnMessenger?.Invoke(_onMessenger);
        Debug.Log(_onMessenger ? "메신저 활성화" : "메신저 비활성화");
    }

    private void ToggleMic()
    {
        _micOn = !_micOn;
        MicMute?.Invoke(_micOn);
        Debug.Log(_micOn ? "마이크 킴" : "마이크 끔");
    }

    private void ToggleReady()
    {
        _onReady = !_onReady;
        OnPlayerReady?.Invoke(_onReady);
        Debug.Log(_onReady ? "준비 상태 활성화" : "준비 상태 비활성화");
    }

    private void ToggleGameStart()
    {
        _onGameStart = !_onGameStart;
        OnGameStart?.Invoke(_onGameStart);
        Debug.Log(_onGameStart ? "게임 시작" : "게임 시작 취소");
    }
}
