using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Fusion;

public class SessionUIManager : MonoBehaviour
{
    public static SessionUIManager Instance { get; private set; }
    public Canvas createRoomCanvas;
    public Button createRoomButton;
    public TMP_InputField sessionNameInput;
    public TMP_InputField sessionIntroductionInput;
    public Button startSessionButton;
    public Button backButton;
    public GameObject sessionPrefab;
    public Transform sessionListParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        createRoomButton.onClick.AddListener(ToggleCreateRoomCanvas);
        startSessionButton.onClick.AddListener(CreateSession);
        backButton.onClick.AddListener(ToggleCreateRoomCanvas);
    }

    private async void CreateSession()
    {
        string sessionName = sessionNameInput.text;

        if (string.IsNullOrEmpty(sessionName))
        {
            Debug.LogWarning("세션 이름이 비어있습니다.");
            return;
        }

        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(3));
        
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 4,
            Scene = sceneInfo,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "description", sessionIntroductionInput.text }
            }
        };

        await NetworkManager.Instance.ShutdownRunner();
        await NetworkManager.Instance.RunnerStart(startArgs);
    }

    private void ToggleCreateRoomCanvas()
    {
        createRoomCanvas.enabled = !createRoomCanvas.enabled;
    }

    // 세션 목록 UI 업데이트
    public void UpdateSessionList(List<SessionInfo> sessionList)
    {
        foreach (Transform child in sessionListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var session in sessionList)
        {
            GameObject sessionButton = Instantiate(sessionPrefab, sessionListParent);
            TMP_Text sessionText = sessionButton.GetComponentInChildren<TMP_Text>();
            sessionText.text = $"{session.Name} ({session.PlayerCount}/{session.MaxPlayers})";

            Button button = sessionButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnJoinSession(session));
        }
    }

    private async void OnJoinSession(SessionInfo session)
    {
        Debug.Log($"Joining session: {session.Name}");

        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = session.Name
        };

        await NetworkManager.Instance.ShutdownRunner();
        await NetworkManager.Instance.RunnerStart(startArgs);
    }
}