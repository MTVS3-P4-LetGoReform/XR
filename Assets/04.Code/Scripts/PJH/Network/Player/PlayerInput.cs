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
    public static Action<bool> OnMessenger;

    private bool _mouseOn;
    private bool _chatOn;
    private bool _micOn;
    private bool _onReady;
    private bool _onGameStart;
    private bool _onMessenger = true;

    private const string ParkScene = "Alpha_PublicParkScene";
    private const string GameScene = "Alpha_PlayScene";
    private const string PersonalScene = "Alpha_PersonalScene";
    
    private PlayerStatus PlayerStatus { get; set; }

    public override void Spawned()
    {
        PlayerStatus = GetComponentInParent<PlayerStatus>();
        if (PlayerStatus == null)
        {
            Debug.LogError("PlayerStatus가 부모 객체에 없습니다.");
        }

        OnMessenger += MessengerStatus;
    }

    private void Update()
    {
        HandleGeneralInput();
        HandleSceneSpecificInput();
    }

    public static void PlayerLockCamera(bool isActive)
    {
        OnMouse?.Invoke(isActive);
    }

    private void HandleGeneralInput()
    {
        

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

        if (sceneName is ParkScene or PersonalScene)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("P 키 입력");
                ToggleMessenger();
            }
        }
        else if (sceneName == GameScene)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("V 키 입력");
                ToggleMic();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("F1 키 입력");
                ToggleReady();
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.Log("F2 키 입력");
                ToggleGameStart();
            }
//#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                Debug.Log("= 키 입력, 보상 제공");
                PlayerStatus?.Reword(true);
            }
//#endif
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

    public void ToggleMessenger()
    {
        OnMessenger?.Invoke(_onMessenger);
    }

    private void MessengerStatus(bool active)
    {
        Debug.Log(!active ? "메신저 활성화" : "메신저 비활성화");
        _onMessenger = !active;
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
