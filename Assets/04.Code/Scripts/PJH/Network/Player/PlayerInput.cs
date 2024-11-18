using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : NetworkBehaviour
{
    public static event Action<bool> OnChat;
    public static event Action<bool> MicMute;
    public static event Action<bool> OnPlayerReady;
    public static event Action<bool> OnGameStart;
    public static event Action<bool> OnMessenger;

    public PlayerStatus PlayerStatus { get; private set; }
    
    private string mainSceneName = "Alpha_PublicParkScene";
    private string gameSceneName = "Alpha_PlayScene";
    
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
            OnChat?.Invoke(true); // Chat 활성화
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnChat?.Invoke(false); // Chat 비활성화
        }
    }

    private void HandleSceneSpecificInput()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "mainSceneName")
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("P 키 입력");
                OnMessenger?.Invoke(true); // Messenger 활성화
            }
        }
        else if (sceneName == "GameScene")
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("V 키 입력");
                MicMute?.Invoke(true); // 마이크 활성화
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("F1 키 입력");
                OnPlayerReady?.Invoke(true); // 준비 상태 활성화
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.Log("F2 키 입력");
                OnGameStart?.Invoke(true); // 게임 시작 활성화
            }
        }
    }
}
