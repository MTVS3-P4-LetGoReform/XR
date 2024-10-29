using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    // 세션 목록 UI 업데이트
    public void UpdateSessionList(List<SessionInfo> sessionList)
    {
        foreach (Transform child in sessionListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var session in sessionList)
        {
            if (session.Name == "공용 세션")
            {
                return;
            }
            string url = GetImage(session); // 추후 AI 이미지를 불러올때 프롬프트를 사용해서 불러오기 URL 가져와서 이미지 출력
            
            GameObject sessionButton = Instantiate(sessionPrefab, sessionListParent);
            TMP_Text sessionText = sessionButton.GetComponentInChildren<TMP_Text>();
            sessionText.text = $"{session.Name} <br>{session.PlayerCount}/{session.MaxPlayers}";
            
            Button button = sessionButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnJoinSession(session));
        }
    }

    private void ToggleCreateRoomCanvas()
    {
        createRoomCanvas.enabled = !createRoomCanvas.enabled;
    }
    
    private string GetImage(SessionInfo session)
    {
        string prompt = "";
        if (session.Properties.TryGetValue("Prompt", out var sessionDescription))
        {
            prompt = sessionDescription;
        }
        else
        {
            Debug.LogWarning($"{session.Name}: 이미지를 불러오지 못했습니다.");
        }
        Debug.Log("결괏값: " + prompt);
        return prompt;
    }

    private async void CreateSession()
    {
        string sessionName = sessionNameInput.text;

        if (string.IsNullOrEmpty(sessionName))
        {
            Debug.LogWarning("세션 이름이 비어있습니다.");
            return;
        }

        var playSceneNum = 1;
        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(playSceneNum)); // PlayScene으로 이동
        
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 4,
            //Scene = sceneInfo,
            /*SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "Prompt", sessionIntroductionInput.text }
            }*/
        };

        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(startArgs,playSceneNum);
    }
    
    private async void OnJoinSession(SessionInfo session)
    {
        Debug.Log($"Joining session: {session.Name}");

        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = session.Name
        };

        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(startArgs,1);
    }
}