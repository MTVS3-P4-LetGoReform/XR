using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Fusion;
using UnityEngine.SceneManagement;

public class SessionUIManager : MonoBehaviour
{
    public static SessionUIManager Instance { get; private set; }
    
    public GameObject roomListPanel;
    public GameObject createRoomPanel;
    
    public TMP_InputField sessionNameInput;
    public TMP_InputField sessionPromptInput;
    
    public Transform sessionListParent;
    public GameObject sessionPrefab;
    
    public Button create;
    public Button join;
    
    public Button roomListBack;
    
    public Button createRoomBack;
    public Button createRoomRecreate;
    public Button createRoomStart;

    public GameObject test;

    public WebApiData webApiData;
    public WebCommManager _webCommManager;
    
   
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
        create.onClick.AddListener(ActiveCreateRoom);
        join.onClick.AddListener(ActiveRoomList);
        
        roomListBack.onClick.AddListener(OffRoomList);
        
        createRoomBack.onClick.AddListener(OffCreateRoom);
        //createRoomRecreate.onClick.AddListener(ImageCraft);
        //createRoomStart.onClick.AddListener(CreatePlaySession);
    }

    // private void ImageCraft()
    // {
    //     test.SetActive(true);
    // }
    //

    // 세션 목록 UI 업데이트
    public void UpdateSessionList(List<SessionInfo> sessionList)
    {
        // 기존 목록 삭제 같은 목록이 겹치는걸 방지함.
        foreach (Transform child in sessionListParent)
        {
            Destroy(child.gameObject);
        }

        // 세션 정보를 불러옴
        foreach (var session in sessionList)
        {
            if (session.Name == "공용 세션")
            {
                return;
            }
            
            string check = CheckSession(session);
            if (check != null)
            {
                return;
            }
            
            string url = GetImage(session); // 추후 AI 이미지를 불러올때 프롬프트를 사용해서 불러오기 URL 가져와서 이미지 출력
            
            //목록 생성
            GameObject sessionButton = Instantiate(sessionPrefab, sessionListParent);
            TMP_Text sessionText = sessionButton.GetComponentInChildren<TMP_Text>();
            sessionText.text = $"{session.Name} <br>{session.PlayerCount}/{session.MaxPlayers}";
            
            Button button = sessionButton.GetComponent<Button>();
            button.onClick.AddListener(() => JoinPlaySession(session));
        }
    }

    private string CheckSession(SessionInfo session)
    {
        var userId = "";
        if (session.Properties.TryGetValue("UserId", out var id))
        {
            userId = id;
        }
        else
        {
            userId = null;
        }
        Debug.Log("결괏값: " + userId);
        return userId;
    }
    private string GetImage(SessionInfo session)
    {
        string ImageUrl = "";
        if (session.Properties.TryGetValue("ImageUrl", out var sessionDescription))
        {
            ImageUrl = sessionDescription;
        }
        else
        {
            Debug.LogWarning($"{session.Name}: 이미지를 불러오지 못했습니다.");
        }
        Debug.Log("결괏값: " + ImageUrl);
        return ImageUrl;
    }
    public async void CreatePlaySession()
    {
        Debug.Log("SeissionUIManager : CreatePlaySession()");
        string sessionName = sessionNameInput.text;
        Debug.Log("SeissionUIManager : flag1");
        if (string.IsNullOrEmpty(sessionName))
        {
            Debug.LogWarning("세션 이름이 비어있습니다.");
            return;
        }
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 4,
            //Scene = sceneInfo,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "Prompt", sessionPromptInput.text },
                { "ImageUrl", sessionPromptInput.text },
                {"ModelId", webApiData.ModelId},
                {"ModelName", webApiData.ModelName}
            }
        };

        Debug.Log($"SessionUIManager : ModelId - {startArgs.SessionProperties["ModelId"]}");
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(startArgs,2);
    }
    
    private async void JoinPlaySession(SessionInfo session)
    {
        string ModelName = "";
        if (session.Properties.TryGetValue("ModelName", out var sessionDescription))
        {
            ModelName = sessionDescription;
            webApiData.ModelName = sessionDescription;
            _webCommManager.JoinWebComm(ModelName);
        }
        else
        {
            Debug.LogWarning($"{session.Name}: 모델을 불러오지 못했습니다.");
        }
        
        Debug.Log($"Joining session: {session.Name}");
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = session.Name,
            //Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(""))
        };

        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(startArgs,2);
    }
    
    public void ActiveRoomList()
    {
        roomListPanel.SetActive(true);
    }
    
    public void OffRoomList()
    {
        roomListPanel.SetActive(false);
    }

    public void ActiveCreateRoom()
    {
        createRoomPanel.SetActive(true);
    }
    
    public void OffCreateRoom()
    {
        createRoomPanel.SetActive(false);
    }
}